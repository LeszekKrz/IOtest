using YouTubeV2.Application.DTO.UserDTOS;

namespace YouTubeV2.Application.DTO.SearchDTOS
{
    public record SearchResultsDto(IReadOnlyList<UserDto> users);
}
