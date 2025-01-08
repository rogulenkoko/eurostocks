using EuroStocks.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace EuroStocks.Infrastructure;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<ProductImage> Blogs { get; set; }
    
    public DbSet<Product> Posts { get; set; }
}