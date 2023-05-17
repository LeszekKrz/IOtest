namespace YouTubeV2.Application.Services.BlobServices
{
    public record class BlobFile(Stream Content, string ContentType);
}
