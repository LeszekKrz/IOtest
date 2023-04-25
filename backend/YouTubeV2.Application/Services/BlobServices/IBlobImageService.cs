namespace YouTubeV2.Application.Services.BlobServices
{
    public interface IBlobImageService
    {
        Task UploadProfilePictureAsync(string base64Content, string fileName, CancellationToken cancellationToken = default);

        Uri GetProfilePicture(string fileName);

        Task UploadVideoThumbnailAsync(string base64Content, string fileName, CancellationToken cancellationToken = default);

        Uri GetVideoThumbnail(string fileName);

        Task DeleteThumbnailAsync(string fileName, CancellationToken cancellationToken = default);
    }
}
