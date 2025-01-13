using Amazon.S3;
using Amazon.S3.Model;
using EuroStock.Domain.Services.Abstract;
using Polly;

namespace EuroStocks.Infrastructure.Storage;

public class S3StorageService(IAmazonS3 s3Client) : IStorageService
{
    private const string BucketName = "default";
    
    public async Task<bool> UploadFileAsync(Guid merchantId, Guid id, Stream fileStream, CancellationToken cancellationToken = default)
    {
        try
        {
            await CreateBucketIfNotExistsAsync();
        
            var key = GetPath(merchantId.ToString(), id);
            var putRequest = new PutObjectRequest
            {
                BucketName = BucketName,
                Key = key,
                InputStream = fileStream,
                ContentType = "image/jpeg"
            };
        
            var policy = Policy.Handle<Exception>().WaitAndRetryAsync(3, i => TimeSpan.FromMilliseconds(300));
            await policy.ExecuteAsync(() => s3Client.PutObjectAsync(putRequest, cancellationToken));
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<Stream> GetFileAsync(Guid merchantId, Guid id, CancellationToken cancellationToken = default)
    {
        var key = GetPath(merchantId.ToString(), id);
        var getRequest = new GetObjectRequest
        {
            BucketName = BucketName,
            Key = key
        };

        var response = await s3Client.GetObjectAsync(getRequest, cancellationToken);
        return response.ResponseStream;
    }

    private async Task CreateBucketIfNotExistsAsync()
    {
        var buckets = await s3Client.ListBucketsAsync();
        if (buckets.Buckets.Exists(b => b.BucketName == BucketName)) return;

        await s3Client.PutBucketAsync(new PutBucketRequest
        {
            BucketName = BucketName,
            BucketRegionName = s3Client.Config.AuthenticationRegion,
        });
    }

    private static string GetPath(string container, Guid imageId)
    {
        return $"{container}/{imageId}.jpeg";
    }
}