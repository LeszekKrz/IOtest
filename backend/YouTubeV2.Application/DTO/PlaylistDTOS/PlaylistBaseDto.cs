using YouTubeV2.Api.Enums;

namespace YouTubeV2.Application.DTO.PlaylistDTOS
{
    public record PlaylistBaseDto(string name, string id, Visibility visibility);
}
