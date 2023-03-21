namespace YouTubeV2.Application.Configurations
{
    public class BlobStorageConfig
    {
        public string UserAvatarsContainerName { get; init; } = null!;
        public string VideoThumbnailsContainerName { get; init; } = null!;
    }
}
