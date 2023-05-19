namespace YouTubeV2.Application.DTO.VideoDTOS
{
    public record VideoBaseDto(
        string id,
        string title,
        string duration,
        string thumbnail,
        string description,
        string uploadDate,
        int viewCount);
}
