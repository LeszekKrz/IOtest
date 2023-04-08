using YouTubeV2.Api.Enums;

namespace YouTubeV2.Application.DTO
{
    public record VideoMetadataPostDTO(
        string title,
        string description,
        string thumbnail,
        IReadOnlyCollection<string> tags,
        Visibility visibility);
}
