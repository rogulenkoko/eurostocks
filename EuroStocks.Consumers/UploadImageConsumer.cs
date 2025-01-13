using EasyNetQ.AutoSubscribe;
using EuroStock.Domain.Entities;
using EuroStock.Domain.Models;
using EuroStock.Domain.Services.Abstract;
using EuroStock.Domain.SignalR;
using Microsoft.AspNetCore.SignalR;

namespace EuroStocks.Consumers;

public class UploadImageConsumer(
    IHttpClientFactory httpClientFactory,
    IStorageService storageService,
    IProductService productService,
    IHubContext<AppHub, IAppHubClient> hubContext) : IConsumeAsync<UploadImageMessage>
{
    public async Task ConsumeAsync(UploadImageMessage message, CancellationToken cancellationToken = default)
    {
        try
        {
            using var httpClient = httpClientFactory.CreateClient();
            using var response = await httpClient.GetAsync(message.ImageUrl, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await ProcessFail(message);
                return;
            }

            await using var content = await response.Content.ReadAsStreamAsync(cancellationToken);
            await storageService.UploadFileAsync(message.MerchantId, message.ImageId, content, cancellationToken);
            await productService.MarkImagesAsSaved(ImageStatus.Saved, message.ImageId);

            await hubContext.Clients.Group($"{message.MerchantId}").UploadImage(new UploadImageResult
            {
                ImageId = message.ImageId,
            });
        }
        catch (Exception)
        {
            await ProcessFail(message);
        }
    }

    private async Task ProcessFail(UploadImageMessage message)
    {
        await hubContext.Clients.Group($"{message.MerchantId}").UploadImage(new UploadImageResult
        {
            ImageId = message.ImageId,
            ImageSource = message.ImageUrl,
            Error = $"Unable to download an image by {message.ImageUrl}",
        });
        
        await productService.MarkImagesAsSaved(ImageStatus.Failed, message.ImageId);
    }
}