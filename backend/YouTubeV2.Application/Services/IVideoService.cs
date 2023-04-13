using YouTubeV2.Application.DTO;
using YouTubeV2.Application.Model;

namespace YouTubeV2.Application.Services
{
    public interface IVideoService
    {
        Task<Guid> AddVideoMetadataAsync(VideoMetadataPostDTO videoMetadata, User user, CancellationToken cancellationToken = default);

        Task<VideoMetadataDto> GetVideoMetadataAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
