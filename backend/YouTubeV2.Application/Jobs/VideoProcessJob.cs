namespace YouTubeV2.Application.Jobs
{
    public record class VideoProcessJob(Guid VideoId, Stream VideoStream, string Extension);
}
