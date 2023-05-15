using YouTubeV2.Application.DTO.UserDTOS;
using YouTubeV2.Application.DTO.VideoMetadataDTOS;

namespace YouTubeV2.Application.DTO.SearchDTOS
{
    public record SearchResultsDto(IReadOnlyList<VideoMetadataDto> videos, IReadOnlyList<UserDto> users);
}
