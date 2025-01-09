using EasyNetQ;
using EuroStock.Domain.Services.Abstract;
using Microsoft.Extensions.Logging;
using Polly;

namespace EuroStocks.Infrastructure.Messaging
{
    public class MessagePublisher(IBus bus, ILogger<MessagePublisher> logger) : IMessagePublisher
    {
        public async Task Publish<T>(T message, int attempts)
            where T : class
        {
            try
            {
                var attempt = 0;
                var policy = Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(
                        attempts,
                        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        (exception, _) => logger.LogWarning(
                            "Unsuccessful publish attempt ({Message}) Exception: {Exception} Attempt: {Attempt}",
                            message,
                            exception,
                            ++attempt));

                await policy.ExecuteAsync(async () =>
                {
                    logger.LogTrace("Sending the message ({Message})", message);
                    await bus.PubSub.PublishAsync(message);
                    logger.LogInformation("Sent the message ({Message})", message);
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to publish the message ({Message}) after {Attempts} attempts", message, attempts);
                throw;
            }
        }
    }
}