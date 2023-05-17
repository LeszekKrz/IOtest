using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using YouTubeV2.Application.Configurations.BlobStorage;

namespace YouTubeV2.Application.Services.BlobServices
{
    public class BlobVideoService : IBlobVideoService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobStorageVideosConfig _blobStorageConfig;

        public BlobVideoService(BlobServiceClient blobServiceClient, IOptions<BlobStorageVideosConfig> blobStorageConfig)
        {
            _blobServiceClient = blobServiceClient;
            _blobStorageConfig = blobStorageConfig.Value;
        }

        public async Task<Stream> GetVideoAsync(string fileName, CancellationToken cancellationToken = default)
        {
            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(_blobStorageConfig.VideosContainerName);
            BlobClient blobClient = blobContainerClient.GetBlobClient(fileName.ToLower());

            if (!(await blobClient.ExistsAsync(cancellationToken)).Value)
                throw new FileNotFoundException($"There is no video with fileName {fileName}");

            return await blobClient.OpenReadAsync(false, cancellationToken: cancellationToken);
        }

        public async Task UploadVideoAsync(string fileName, Stream videoStream, CancellationToken cancellationToken = default)
        {
            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(_blobStorageConfig.VideosContainerName);
            BlobClient blobClient = blobContainerClient.GetBlobClient(fileName.ToLower());
            await blobClient.UploadAsync(
                videoStream,
                new BlobUploadOptions { HttpHeaders = new BlobHttpHeaders { ContentType = "video/mp4" } },
                cancellationToken);
        }
    }
}
