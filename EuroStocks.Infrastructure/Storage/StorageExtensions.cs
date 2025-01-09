using Amazon.S3;
using EuroStock.Domain.Services.Abstract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EuroStocks.Infrastructure.Storage;

public static class StorageExtensions
{
    public static IHostApplicationBuilder AddStorage(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IAmazonS3>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var awsOptions = new AmazonS3Config
            {
                ServiceURL = configuration["S3:ServiceURL"],
                UseHttp = true,
                AuthenticationRegion = configuration["S3:Region"],
                ForcePathStyle = true,
            };

            return new AmazonS3Client(
                configuration["S3:AccessKey"],
                configuration["S3:SecretKey"],
                awsOptions
            );
        });
        
        builder.Services.AddScoped<IStorageService, S3StorageService>();

        return builder;
    }
}