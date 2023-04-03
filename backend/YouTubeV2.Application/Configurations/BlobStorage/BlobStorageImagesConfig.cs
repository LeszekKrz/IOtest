namespace YouTubeV2.Application.Configurations.BlobStorage
{
    public class BlobStorageImagesConfig
    {
        public string UserAvatarsContainerName { get; init; } = null!;
        public string VideoThumbnailsContainerName { get; init; } = null!;
    }
}
