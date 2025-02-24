using EuroStock.Domain.Entities;
using EuroStock.Domain.Models;

namespace EuroStock.Domain.Services.Abstract;

public interface IProductService
{
    Task<Guid> CreateOrUpdateProduct(UpdateProductRequest productModel);

    Task<ProductModel> GetProduct(Guid id, Guid merchantId);
    
    Task UpdateImages(UpdateProductImagesRequest request);

    Task MarkImagesAsSaved(ImageStatus status, params Guid[] imagesIds);
}