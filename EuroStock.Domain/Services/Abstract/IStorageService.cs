namespace EuroStock.Domain.Services.Abstract;

public interface IStorageService
{
    Task<Guid> UploadFileAsync(string userId, Stream fileStream, CancellationToken cancellationToken = default);

    Task<Stream> GetFileAsync(string userId, Guid id, CancellationToken cancellationToken = default);
}