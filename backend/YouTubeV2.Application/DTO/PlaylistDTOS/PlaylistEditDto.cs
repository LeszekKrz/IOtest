using YouTubeV2.Api.Enums;

namespace YouTubeV2.Application.DTO.PlaylistDTOS
{
    public record PlaylistEditDto(string name, Visibility visibility);
}
