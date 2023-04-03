using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using YouTubeV2.Application.Configurations.BlobStorage;

namespace YouTubeV2.Application.Services.AzureServices.BlobServices
{
    public class BlobImageService : IBlobImageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobStorageImagesConfig _blobStorageConfig;

        public BlobImageService(BlobServiceClient blobServiceClient, IOptions<BlobStorageImagesConfig> blobStorageConfig)
        {
            _blobServiceClient = blobServiceClient;
            _blobStorageConfig = blobStorageConfig.Value;
        }

        public Uri GetProfilePicture(string fileName) => GetImageUrl(fileName, _blobStorageConfig.UserAvatarsContainerName);

        public async Task UploadProfilePictureAsync(byte[] bytes, string fileName, CancellationToken cancellationToken = default) =>
            await UploadImageAsync(bytes, fileName, _blobStorageConfig.UserAvatarsContainerName, cancellationToken);

        public Uri GetVideoThumbnail(string fileName) => GetImageUrl(fileName, _blobStorageConfig.VideoThumbnailsContainerName);

        public async Task UploadVideoThumbnailAsync(byte[] bytes, string fileName, CancellationToken cancellationToken = default) =>
            await UploadImageAsync(bytes, fileName, _blobStorageConfig.VideoThumbnailsContainerName, cancellationToken);

        private Uri GetImageUrl(string fileName, string blobContainerName)
        {
            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(blobContainerName);

            return blobContainerClient.GetBlobClient(fileName).Uri;
        }

        private async Task UploadImageAsync(byte[] bytes, string fileName, string blobContainerName, CancellationToken cancellationToken = default)
        {
            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(blobContainerName);
            BlobClient blobClient =  blobContainerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(
                new BinaryData(bytes),
                new BlobUploadOptions { HttpHeaders = new BlobHttpHeaders { ContentType = "image/png" } },
                cancellationToken);
        }
    }
}
