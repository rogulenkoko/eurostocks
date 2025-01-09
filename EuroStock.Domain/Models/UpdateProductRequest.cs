namespace EuroStock.Domain.Models;

public class UpdateProductRequest : BaseRequest
{
    public Guid? ProductId { get; set; }
    
    public string Name { get; set; }
}