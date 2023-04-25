using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using YouTubeV2.Application.Configurations.BlobStorage;
using YouTubeV2.Application.Utils;

namespace YouTubeV2.Application.Services.BlobServices
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

        public async Task UploadProfilePictureAsync(string base64Content, string fileName, CancellationToken cancellationToken = default) =>
            await UploadImageAsync(base64Content, fileName, _blobStorageConfig.UserAvatarsContainerName, cancellationToken);

        public async Task DeleteProfilePictureAsync(string fileName, CancellationToken cancellationToken = default) =>
            await DeleteImageAsync(fileName, _blobStorageConfig.UserAvatarsContainerName, cancellationToken);

        public async Task DeleteThumbnailAsync(string fileName, CancellationToken cancellationToken = default) =>
          await DeleteImageAsync(fileName, _blobStorageConfig.VideoThumbnailsContainerName, cancellationToken);

        public Uri GetVideoThumbnail(string fileName) => GetImageUrl(fileName, _blobStorageConfig.VideoThumbnailsContainerName);

        public async Task UploadVideoThumbnailAsync(string base64Content, string fileName, CancellationToken cancellationToken = default) =>
            await UploadImageAsync(base64Content, fileName, _blobStorageConfig.VideoThumbnailsContainerName, cancellationToken);
 
        private Uri GetImageUrl(string fileName, string blobContainerName)
        {
            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(blobContainerName);

            return blobContainerClient.GetBlobClient(fileName.ToLower()).Uri;
        }

        private async Task UploadImageAsync(string base64Content, string fileName, string blobContainerName, CancellationToken cancellationToken = default)
        {
            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(blobContainerName);
            BlobClient blobClient = blobContainerClient.GetBlobClient(fileName.ToLower());

            string imageFormat = base64Content.GetImageFormat();
            string imageData = base64Content.GetImageData();

            await using MemoryStream memoryStream = new(Convert.FromBase64String(imageData));
            await blobClient.UploadAsync(memoryStream, new BlobHttpHeaders { ContentType = imageFormat }, cancellationToken: cancellationToken);
        }

        private async Task DeleteImageAsync(string fileName, string blobContainerName, CancellationToken cancellationToken = default)
        {
            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(blobContainerName);
            await blobContainerClient.DeleteBlobIfExistsAsync(fileName.ToLower(), DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: cancellationToken);
        }
    }
}
