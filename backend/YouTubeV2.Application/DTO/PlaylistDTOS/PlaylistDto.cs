using YouTubeV2.Api.Enums;
using YouTubeV2.Application.DTO.VideoMetadataDTOS;

namespace YouTubeV2.Application.DTO.PlaylistDTOS
{
    public record PlaylistDto(string name,
                              Visibility visibility,
                              IEnumerable<VideoMetadataDto> videos,
                              string authorId,
                              string authorNickname);
}
