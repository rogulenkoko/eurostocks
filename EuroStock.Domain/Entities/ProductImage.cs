using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EuroStock.Domain.Entities;

[Table("product_image")]
public class ProductImage : Entity
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }
    
    [Column("product_id")]
    public Guid ProductId { get; set; }
    
    public virtual Product? Product { get; set; }
    
    [Column("file_name")]
    [MaxLength(255)]
    public string? FileName { get; set; }
    
    [Column("ext")]
    [MaxLength(255)]
    public string? Extension { get; set; }
    
    [Column("source_url")]
    public string? SourceUrl { get; set; }
    
    [Column("sequence_number")]
    public int? SequenceNumber { get; set; }
    
    [Column("is_deleted")]
    public bool IsDeleted { get; set; }

    [Column("status")]
    public ImageStatus Status { get; set; }
}