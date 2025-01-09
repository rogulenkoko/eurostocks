using System.Text.Json.Serialization;

namespace EuroStock.Domain.Models;

public abstract class BaseRequest
{
    [JsonIgnore]
    public Guid UserId { get; set; }
    
    [JsonIgnore]
    public Guid MerchantId { get; set; }
}