using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using EuroStocks.Domain.Services.Abstract;

namespace EuroStocks.Domain.Services;

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
        await s3Client.PutObjectAsync(putRequest, cancellationToken);
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
    
    // public async Task<Guid> PutPhoto(byte[] data, string container, CancellationToken cancellationToken = default)
    // {
    //     var id = Guid.NewGuid();
    //     var name = GetPath(container, id);
    //
    //     var response = await s3Client.ListBucketsAsync(cancellationToken);
    //     
    //     var policy = Policy.Handle<Exception>().WaitAndRetryAsync(3, i => TimeSpan.FromMilliseconds(300));
    //     await policy.ExecuteAsync(() => _transferUtility.UploadAsync(new MemoryStream(data), BucketName, name, cancellationToken));
    //
    //     return id;
    // }

    private static string GetPath(string container, Guid imageId)
    {
        return $"{container}/{imageId}.jpeg";
    }
}