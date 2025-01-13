using System.ComponentModel.DataAnnotations.Schema;

namespace EuroStock.Domain.Entities;

public abstract class Entity
{
    [Column("merchant_id")]
    public Guid MerchantId { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [Column("created_by")]
    public Guid CreatedBy { get; set; }
}