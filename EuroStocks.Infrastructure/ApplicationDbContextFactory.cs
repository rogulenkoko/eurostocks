using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EuroStocks.Infrastructure;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        // var connectionString = "User ID=eurostocks;Password=eurostocks;Server=127.0.0.1;Port=49239;Database=eurostocks;Pooling=true;";
        var connectionString = "User ID=eurostocks;Password=eurostocks;Server=127.0.0.1;Port=5432;Database=eurostocks;Pooling=true;";
        optionsBuilder
            .UseNpgsql(connectionString);
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}