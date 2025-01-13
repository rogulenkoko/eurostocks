using EuroStock.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EuroStocks.Infrastructure;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options), IApplicationDbContext
{
    public DbSet<ProductImage> ProductImages { get; set; }
    
    public DbSet<Product> Products { get; set; }
    
    public Task SaveChangesAsync()
    {
        return base.SaveChangesAsync();
    }
}