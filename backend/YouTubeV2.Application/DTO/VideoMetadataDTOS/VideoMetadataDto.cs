namespace YouTubeV2.Application.DTO.VideoMetadataDTOS
{
    public record VideoMetadataDto(
        string id,
        string title,
        string description,
        string thumbnail,
        string authorId,
        string authorNickname,
        int viewCount,
        IReadOnlyCollection<string> tags,
        string visibility,
        string processingProgress,
        DateTime uploadDate,
        DateTime editDate,
        string duration);
}
