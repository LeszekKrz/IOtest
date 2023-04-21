namespace YouTubeV2.Application.Providers
{
    public interface IDateTimeProvider
    {
        DateTimeOffset UtcNow { get; }
    }
}
