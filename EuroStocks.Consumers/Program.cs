using EuroStock.Domain.Services.Abstract;
using EuroStocks.Consumers;
using EuroStocks.Infrastructure;
using EuroStocks.Infrastructure.Messaging;
using EuroStocks.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection"));
});

builder.AddMessaging();
builder.AddMessagingSubscribers("consumers", typeof(UploadImageConsumer).Assembly);
builder.AddStorage();
builder.Services.AddHttpClient();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseConsumers(typeof(UploadImageConsumer).Assembly);

app.Run();