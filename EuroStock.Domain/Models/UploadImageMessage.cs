namespace EuroStock.Domain.Models;

public class UploadImageMessage
{
    public Guid ImageId { get; set; }
    
    public Guid MerchantId { get; set; }
    
    public DateTime Date { get; set; }
    
    public string ImageUrl { get; set; }
}