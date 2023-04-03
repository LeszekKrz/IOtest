namespace YouTubeV2.Application.Services.AzureServices.BlobServices
{
    public interface IBlobVideoService
    {
        Task<Stream> GetVideoAsync(string fileName, CancellationToken cancellationToken = default);

        Task UploadVideoAsync(string fileName, Stream videoStream, CancellationToken cancellationToken = default);
    }
}
