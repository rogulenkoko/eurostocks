using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EuroStocks.Infrastructure.Entities;

[Table("product")]
public class Product : Entity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("name")]
    [MaxLength(255)]
    public string Name { get; set; }
    
    public virtual List<ProductImage> ProductImages { get; set; }
}