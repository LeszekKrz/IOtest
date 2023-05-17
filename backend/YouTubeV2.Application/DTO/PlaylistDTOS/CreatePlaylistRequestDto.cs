using YouTubeV2.Api.Enums;

namespace YouTubeV2.Application.DTO.PlaylistDTOS
{
    public record CreatePlaylistRequestDto(string name, Visibility visibility);
}
