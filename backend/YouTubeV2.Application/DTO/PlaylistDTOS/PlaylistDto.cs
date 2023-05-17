using YouTubeV2.Api.Enums;
using YouTubeV2.Application.DTO.VideoDTOS;

namespace YouTubeV2.Application.DTO.PlaylistDTOS
{
    public record PlaylistDto(string name, Visibility visibility, IEnumerable<VideoBaseDto> videos);
}
