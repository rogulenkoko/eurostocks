using EuroStock.Domain.Exceptions;
using EuroStock.Domain.Models;
using EuroStock.Domain.Services.Abstract;
using EuroStocks.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace EuroStocks.Infrastructure.Services;

public class ProductService(
    IApplicationDbContext dbContext,
    IStorageService storageService,
    IMessagePublisher messagePublisher) : IProductService
{
    public async Task<Guid> CreateOrUpdateProduct(UpdateProductRequest productModel)
    {
        Product? product;

        if (productModel.ProductId.HasValue)
        {
            product = await dbContext.Products.Include(x => x.ProductImages)
                .FirstOrDefaultAsync(x => x.Id == productModel.ProductId &&
                                                 x.MerchantId == productModel.MerchantId);

            if (product == null)
            {
                throw new NotFoundException();
            }
        }
        else
        {
            product = new Product
            {
                MerchantId = productModel.MerchantId,
                CreatedBy = productModel.UserId,
                CreatedAt = DateTime.UtcNow,
            };

            dbContext.Products.Add(product);
        }

        product.Name = productModel.Name;

        await dbContext.SaveChangesAsync();

        return product.Id;
    }

    public async Task<ProductModel> GetProduct(Guid id, Guid merchantId)
    {
        var product = await dbContext.Products.Include(x => x.ProductImages)
            .FirstOrDefaultAsync(x => x.Id == id &&
                                      x.MerchantId == merchantId);

        if (product == null)
        {
            throw new NotFoundException();
        }

        return new ProductModel
        {
            Id = product.Id,
            Name = product.Name,
            Images = product.ProductImages.Where(x => !x.IsDeleted).Select(x => x.Id).ToList(),
        };
    }

    public async Task UpdateImages(UpdateProductImagesRequest request)
    {
        var product = await dbContext.Products.Include(x => x.ProductImages)
            .FirstOrDefaultAsync(x => x.Id == request.ProductId &&
                                      x.MerchantId == request.MerchantId);

        if (product == null)
        {
            throw new NotFoundException();
        }
        
        // delete images
        foreach (var deletedImage in product.ProductImages.Where(x => 
                     request.Images.All(y => x.Id != y.Id)))
        {
            deletedImage.IsDeleted = true;
            deletedImage.SequenceNumber = null;
        }

        // update sequence number
        foreach (var image in request.Images.Where(x => x.Id.HasValue))
        {
            var existingImage = product.ProductImages.FirstOrDefault(x => x.Id == image.Id);

            if (existingImage != null)
            {
                existingImage.SequenceNumber = image.SequenceNumber;
            }
        }

        await dbContext.SaveChangesAsync();
        
        // create new images

        var uploadTasks = new List<Task>();
        
        foreach (var image in request.Images.Where(x => x.Id == null))
        {
            var newImage = new ProductImage
            {
                Id = Guid.NewGuid(),
                SequenceNumber = image.SequenceNumber,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.UserId,
                MerchantId = request.MerchantId,
                ProductId = request.ProductId,
                IsDeleted = false,
                SourceUrl = image.FileUrl,
                FileName = image.File?.Name,
                Extension = Path.GetExtension(image.File?.FileName),
            };
            
            dbContext.ProductImages.Add(newImage);

            if (image.File != null)
            {
                var uploadAction = async () =>
                {
                    await using var stream = image.File.OpenReadStream();
                    await storageService.UploadFileAsync(request.MerchantId, newImage.Id, stream);
                };
                uploadTasks.Add(uploadAction.Invoke());
            }
            else
            {
                uploadTasks.Add(messagePublisher.Publish(new UploadImageMessage
                {
                    MerchantId = request.MerchantId,
                    Date = DateTime.UtcNow,
                    ImageId = newImage.Id,
                    ImageUrl = image.FileUrl,
                }));
            }
        }

        await dbContext.SaveChangesAsync();
        
        await Task.WhenAll(uploadTasks);
    }
}