namespace EuroStock.Domain.Models;

public class UploadImageResult
{
    public Guid ImageId { get; set; }
    
    public string? Error { get; set; }

    public bool IsSuccess => Error == null;
}