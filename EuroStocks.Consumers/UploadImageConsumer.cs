using EasyNetQ.AutoSubscribe;
using EuroStock.Domain.Models;
using EuroStock.Domain.Services.Abstract;

namespace EuroStocks.Consumers;

public class UploadImageConsumer(
    IHttpClientFactory httpClientFactory,
    IStorageService storageService) : IConsumeAsync<UploadImageMessage>
{
    public async Task ConsumeAsync(UploadImageMessage message, CancellationToken cancellationToken = default)
    {
        using var httpClient = httpClientFactory.CreateClient();
        using var response = await httpClient.GetAsync(message.ImageUrl, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            // todo process error using webhooks
            return;
        }

        await using var content = await response.Content.ReadAsStreamAsync(cancellationToken);
        await storageService.UploadFileAsync(message.MerchantId, message.ImageId, content, cancellationToken);

        // contentType = response.Content.Headers.ContentType?.MediaType;
        
    }
}