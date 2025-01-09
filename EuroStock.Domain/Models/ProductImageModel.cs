using Microsoft.AspNetCore.Http;

namespace EuroStock.Domain.Models;

public class ProductImageModel
{
    public Guid? Id { get; set; }
    
    public int SequenceNumber { get; set; }
    
    public IFormFile File { get; set; }
    
    public string FileUrl { get; set; }
}