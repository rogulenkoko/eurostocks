using System.Reflection;
using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using EuroStock.Domain.Configurations;
using EuroStock.Domain.Services.Abstract;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EuroStocks.Infrastructure.Messaging;

public static class MessagingExtension
{
    public static IHostApplicationBuilder AddMessaging(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IBus>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var rabbitMqConnection = configuration.GetSection("RabbitMq").Get<RabbitMqConfiguration>();

            return RabbitHutch.CreateBus($"host={rabbitMqConnection.HostName};username={rabbitMqConnection.UserName};password={rabbitMqConnection.Password}");
        });
        
        builder.Services.AddScoped<IMessagePublisher, MessagePublisher>();

        return builder;
    }

    public static IHostApplicationBuilder AddMessagingSubscribers(this IHostApplicationBuilder builder, string subscriptionPrefix, params Assembly[] assemblies)
    {
        builder.Services.AddSingleton<RabbitMqMessageDispatcher>();
        builder.Services.AddSingleton(provider =>
            new AutoSubscriber(provider.GetRequiredService<IBus>(), subscriptionPrefix)
            {
                AutoSubscriberMessageDispatcher = provider.GetRequiredService<RabbitMqMessageDispatcher>(),
            });

        builder.AddConsumers(assemblies);

        return builder;
    }
    
    public static IApplicationBuilder UseConsumers(this IApplicationBuilder app, params Assembly[] assemblies)
    {
        var cancellationToken = CancellationToken.None;
        var subscription = app.ApplicationServices
            .GetService<AutoSubscriber>()
            .SubscribeAsync(assemblies, cancellationToken);

        cancellationToken.Register(async () =>
        {
            (await subscription).Dispose();
        });

        return app;
    }
    
    private static void AddConsumers(this IHostApplicationBuilder builder, params Assembly[] assemblies)
    {
        var providersTypes = assemblies.SelectMany(x => x.GetTypes().Where(
            t => t.IsClass &&
                 t.GetInterfaces()
                     .Any(type =>
                         type.IsGenericType &&
                         type.GetGenericTypeDefinition() == typeof(IConsumeAsync<>))));

        foreach (var providerType in providersTypes)
        {
            builder.Services.AddScoped(providerType);
        }
    }
}