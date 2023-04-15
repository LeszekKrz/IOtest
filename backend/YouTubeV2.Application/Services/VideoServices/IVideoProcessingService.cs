using YouTubeV2.Application.Jobs;

namespace YouTubeV2.Application.Services.VideoServices
{
    public interface IVideoProcessingService
    {
        ValueTask EnqueVideoProcessingJobAsync(VideoProcessJob videoProcessJob);
    }
}
