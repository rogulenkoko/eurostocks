using EuroStock.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EuroStocks.Infrastructure;

public interface IApplicationDbContext
{
    DbSet<ProductImage> ProductImages { get; }
    
    DbSet<Product> Products { get; }
    
    Task SaveChangesAsync();
}