using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EuroStocks.Infrastructure.Entities;

[Table("product_image")]
public class ProductImage
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }
    
    [Column("product_id")]
    public Guid ProductId { get; set; }
    
    public virtual Product Product { get; set; }
    
    [Column("file_name")]
    [MaxLength(255)]
    public string FileName { get; set; }
    
    [Column("ext")]
    [MaxLength(255)]
    public string Extension { get; set; }
    
    [Column("data")]
    public byte[] Data { get; set; }
    
    [Column("sequence")]
    public int? Sequence { get; set; }
    
    [Column("is_deleted")]
    public bool IsDeleted { get; set; }
    
    [Column("user_id")]
    public Guid UserId { get; set; }
}