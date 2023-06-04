using System.Linq.Expressions;
using YouTubeV2.Application.DTO.VideoDTOS;
using YouTubeV2.Application.DTO.VideoMetadataDTOS;
using YouTubeV2.Application.Enums;
using YouTubeV2.Application.Model;

namespace YouTubeV2.Application.Services.VideoServices
{
    public interface IVideoService
    {
        Task<Guid> AddVideoMetadataAsync(VideoMetadataAddOrUpdateDto videoMetadata, User user, CancellationToken cancellationToken = default);

        Task<Video?> GetVideoByIdAsync(Guid id, CancellationToken cancellationToken = default, params Expression<Func<Video, object>>[] includes);

        Task SetVideoProcessingProgressAsync(Video video, ProcessingProgress processingProgress, CancellationToken cancellationToken = default);

        Task<VideoMetadataDto> GetVideoMetadataAsync(Guid id, CancellationToken cancellationToken = default);

        Task AuthorizeVideoAccessAsync(Guid videoId, string userId, CancellationToken cancellationToken = default);

        Task SetVideoLengthAsync(Video video, double length, CancellationToken cancellationToken = default);

        Task<VideoListDto> GetAllUserVideos(string userId, CancellationToken cancellationToken = default);

        Task<VideoListDto> GetAllAvailableUserVideos(string userId, CancellationToken cancellationToken= default);

        Task<VideoListDto> GetVideosFromSubscriptionsAsync(string userId, CancellationToken cancellationToken = default);

        Task DeleteVideoAsync(Video video, CancellationToken cancellationToken = default);

        Task<int> GetVideoCountAsync(User user, CancellationToken cancellationToke = default);

        Task UpdateVideoMetadataAsync(VideoMetadataAddOrUpdateDto videoMetadata, Video video, CancellationToken cancellationToken = default);
    }
}
