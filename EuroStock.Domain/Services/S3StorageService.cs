using Amazon.S3;
using Amazon.S3.Model;
using EuroStock.Domain.Services.Abstract;
using Polly;

namespace EuroStock.Domain.Services;

public class S3StorageService(IAmazonS3 s3Client) : IStorageService
{
    private const string BucketName = "default";
    
    public async Task<Guid> UploadFileAsync(string userId, Stream fileStream, CancellationToken cancellationToken = default)
    {
        await CreateBucketIfNotExistsAsync();
        
        var id = Guid.NewGuid();
        var key = GetPath(userId, id);
        var putRequest = new PutObjectRequest
        {
            BucketName = BucketName,
            Key = key,
            InputStream = fileStream,
            ContentType = "image/jpeg"
        };
        
        var policy = Policy.Handle<Exception>().WaitAndRetryAsync(3, i => TimeSpan.FromMilliseconds(300));
        await policy.ExecuteAsync(() => s3Client.PutObjectAsync(putRequest, cancellationToken));
        return id;
    }

    public async Task<Stream> GetFileAsync(string userId, Guid id, CancellationToken cancellationToken = default)
    {
        var key = GetPath(userId, id);
        var getRequest = new GetObjectRequest
        {
            BucketName = BucketName,
            Key = key
        };

        var response = await s3Client.GetObjectAsync(getRequest, cancellationToken);
        return response.ResponseStream;
    }

    public async Task CreateBucketIfNotExistsAsync()
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