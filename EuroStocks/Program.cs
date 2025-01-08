using Amazon.S3;
using EuroStocks;
using EuroStocks.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IStorageService = EuroStock.Domain.Services.Abstract.IStorageService;
using S3StorageService = EuroStock.Domain.Services.S3StorageService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection"));
});

builder.Services.AddCors(c =>
{
    c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(i => i.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseRouting();

app.UseHttpsRedirection();

app.RegisterImagesEndpoint();

app.Run();