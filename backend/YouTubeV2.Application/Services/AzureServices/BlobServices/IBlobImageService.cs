namespace YouTubeV2.Application.Services.AzureServices.BlobServices
{
    public interface IBlobImageService
    {
        Task UploadImageAsync(byte[] fileContent, string fileName, string blobContainerName, CancellationToken cancellationToken = default);
        Uri GetImageUrl(string fileName, string blobContainerName);
        Task UploadProfilePictureAsync(byte[] bytes, string fileName, CancellationToken cancellationToken = default);
    }
}
