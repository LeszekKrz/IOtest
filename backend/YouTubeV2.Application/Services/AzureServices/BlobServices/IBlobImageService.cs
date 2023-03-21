using Azure.Storage.Blobs.Models;

namespace YouTubeV2.Application.Services.AzureServices.BlobServices
{
    public interface IBlobImageService
    {
        public Task UploadImageAsync(byte[] bytes, string fileName, string blobContainerName);
        public Uri GetImageUrl(string fileName, string blobContainerName);
    }
}
