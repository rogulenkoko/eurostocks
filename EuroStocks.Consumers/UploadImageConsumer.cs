using EasyNetQ.AutoSubscribe;
using EuroStock.Domain.Models;
using EuroStock.Domain.Services.Abstract;
using EuroStock.Domain.SignalR;
using Microsoft.AspNetCore.SignalR;

namespace EuroStocks.Consumers;

public class UploadImageConsumer(
    IHttpClientFactory httpClientFactory,
    IStorageService storageService,
    IHubContext<AppHub, IAppHubClient> hubContext) : IConsumeAsync<UploadImageMessage>
{
    public async Task ConsumeAsync(UploadImageMessage message, CancellationToken cancellationToken = default)
    {
        using var httpClient = httpClientFactory.CreateClient();
        using var response = await httpClient.GetAsync(message.ImageUrl, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            await hubContext.Clients.Group($"{message.MerchantId}").UploadImage(new UploadImageResult
            {
                ImageId = message.ImageId,
                Error = $"Unable to download an image by {message.ImageUrl}",
            });
            return;
        }

        await using var content = await response.Content.ReadAsStreamAsync(cancellationToken);
        await storageService.UploadFileAsync(message.MerchantId, message.ImageId, content, cancellationToken);

        await hubContext.Clients.Group($"{message.MerchantId}").UploadImage(new UploadImageResult
        {
            ImageId = message.ImageId,
        });
    }
}