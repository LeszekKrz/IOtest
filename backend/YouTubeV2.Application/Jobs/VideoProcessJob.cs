namespace YouTubeV2.Application.Jobs
{
    public record class VideoProcessJob(Guid VideoId, string Path, string Extension);
}
