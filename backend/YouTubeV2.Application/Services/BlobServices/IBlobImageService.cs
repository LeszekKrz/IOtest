namespace YouTubeV2.Application.Services.BlobServices
{
    public interface IBlobImageService
    {
        Task UploadProfilePictureAsync(string base64Content, string fileName, CancellationToken cancellationToken = default);

        Uri GetProfilePictureUrl(string fileName);

        Task UploadVideoThumbnailAsync(string base64Content, string fileName, CancellationToken cancellationToken = default);

        Uri GetVideoThumbnailUrl(string fileName);

        Task DeleteThumbnailAsync(string fileName, CancellationToken cancellationToken = default);

        Task DeleteProfilePictureAsync(string fileName, CancellationToken cancellationToken = default);

        Task<BlobFile> GetImageAsync(string blobContainerName, string fileName, CancellationToken cancellationToken = default);
    }
}
