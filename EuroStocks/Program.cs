using EuroStock.Domain.Configurations;
using EuroStock.Domain.Services.Abstract;
using EuroStock.Domain.SignalR;
using EuroStocks;
using EuroStocks.Infrastructure;
using EuroStocks.Infrastructure.Messaging;
using EuroStocks.Infrastructure.Services;
using EuroStocks.Infrastructure.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection"));
});

builder.Services.AddCors(c =>
{
    c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});


builder.AddMessaging();
builder.AddStorage();

builder.Services.AddScoped<IProductService, ProductService>();

var redisSection = builder.Configuration.GetSection("Redis");
var redisConfiguration = redisSection.Get<RedisConfiguration>();
builder.Services.AddSignalR().AddStackExchangeRedis(redisConfiguration.ConnectionString);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(i => i.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.MapHub<AppHub>("/hubs/app");

app.UseRouting();

app.UseHttpsRedirection();

app.RegisterProductEndpoint();

app.Run();