namespace EuroStock.Domain.Services.Abstract;

public interface IStorageService
{
    Task<bool> UploadFileAsync(Guid merchantId, Guid id, Stream fileStream, CancellationToken cancellationToken = default);

    Task<Stream> GetFileAsync(Guid merchantId, Guid id, CancellationToken cancellationToken = default);
}