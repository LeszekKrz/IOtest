using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using YouTubeV2.Application.Configurations.BlobStorage;
using YouTubeV2.Application.Utils;

namespace YouTubeV2.Application.Services.BlobServices
{
    public class BlobImageService : IBlobImageService
    {
        private const string _defaultImage = "default.png";
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobStorageImagesConfig _blobStorageConfig;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LinkGenerator _linkGenerator;

        public BlobImageService(
            BlobServiceClient blobServiceClient,
            IOptions<BlobStorageImagesConfig> blobStorageConfig,
            IHttpContextAccessor httpContextAccessor,
            LinkGenerator linkGenerator)
        {
            _blobServiceClient = blobServiceClient;
            _blobStorageConfig = blobStorageConfig.Value;
            _httpContextAccessor = httpContextAccessor;
            _linkGenerator = linkGenerator;
        }

        public Uri GetProfilePictureUrl(string fileName) => GetImageUrl(fileName, _blobStorageConfig.UserAvatarsContainerName);

        public async Task UploadProfilePictureAsync(string base64Content, string fileName, CancellationToken cancellationToken = default) =>
            await UploadImageAsync(base64Content, fileName, _blobStorageConfig.UserAvatarsContainerName, cancellationToken);

        public async Task DeleteProfilePictureAsync(string fileName, CancellationToken cancellationToken = default) =>
            await DeleteImageAsync(fileName, _blobStorageConfig.UserAvatarsContainerName, cancellationToken);

        public async Task DeleteThumbnailAsync(string fileName, CancellationToken cancellationToken = default) =>
          await DeleteImageAsync(fileName, _blobStorageConfig.VideoThumbnailsContainerName, cancellationToken);

        public Uri GetVideoThumbnailUrl(string fileName) => GetImageUrl(fileName, _blobStorageConfig.VideoThumbnailsContainerName);

        public async Task UploadVideoThumbnailAsync(string base64Content, string fileName, CancellationToken cancellationToken = default) =>
            await UploadImageAsync(base64Content, fileName, _blobStorageConfig.VideoThumbnailsContainerName, cancellationToken);

        private Uri GetImageUrl(string fileName, string blobContainerName)
        {
            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(blobContainerName);
            if (blobContainerClient.GetBlobClient(fileName.ToLower()).Exists().Value is false)
                fileName = _defaultImage;

            string? uri = _linkGenerator.GetUriByRouteValues(
                httpContext: _httpContextAccessor.HttpContext!,
                routeName: "GetImageAsyncRoute",
                values: new { blobContainerName = blobContainerName, fileName = fileName }
            );

            return new Uri(uri!);
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

        public async Task<BlobFile> GetImageAsync(string blobContainerName, string fileName, CancellationToken cancellationToken = default)
        {
            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(blobContainerName);
            BlobClient blobClient = blobContainerClient.GetBlobClient(fileName.ToLower());

            if (!blobClient.Exists().Value)
                blobClient = blobContainerClient.GetBlobClient(_defaultImage);

            var blobProperties = await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken);

            return new BlobFile(
                await blobClient.OpenReadAsync(false, cancellationToken: cancellationToken),
                blobProperties.Value.ContentType);
        }
    }
}
