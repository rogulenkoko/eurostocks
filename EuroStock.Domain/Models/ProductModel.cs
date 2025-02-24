namespace EuroStock.Domain.Models;

public class ProductModel
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public List<Guid> Images { get; set; }
}