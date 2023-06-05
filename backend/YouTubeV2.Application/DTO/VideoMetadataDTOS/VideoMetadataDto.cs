using YouTubeV2.Api.Enums;
using YouTubeV2.Application.Enums;

namespace YouTubeV2.Application.DTO.VideoMetadataDTOS
{
    public record VideoMetadataDto(
        Guid id,
        string title,
        string description,
        Uri thumbnail,
        string authorId,
        string authorNickname,
        int viewCount,
        IReadOnlyCollection<string> tags,
        Visibility visibility,
        ProcessingProgress processingProgress,
        DateTimeOffset uploadDate,
        DateTimeOffset editDate,
        string duration);
}
