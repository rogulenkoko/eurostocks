namespace EuroStock.Domain.Models;

public class UpdateProductImagesRequest : BaseRequest
{
    public Guid ProductId { get; set; }
    
    public List<ProductImageModel> Images { get; set; }
}