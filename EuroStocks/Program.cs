using Amazon.S3;
using EuroStocks;
using EuroStocks.Domain.Services;
using EuroStocks.Domain.Services.Abstract;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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