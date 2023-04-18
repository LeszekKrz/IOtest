using YouTubeV2.Application.DTO.VideoMetadataDTOS;

namespace YouTubeV2.Application.DTO.VideoDTOS
{
    public record VideoListDto(IReadOnlyCollection<VideoMetadataDto> videos);
}
