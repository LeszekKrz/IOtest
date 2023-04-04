namespace YouTubeV2.Application.DTO
{
    public record VideoListDto(IReadOnlyCollection<VideoMetadataDto> videos);
}
