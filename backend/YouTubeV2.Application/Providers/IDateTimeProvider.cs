namespace YouTubeV2.Application.Providers
{
    public interface IDateTimeProvider
    {
        public DateTimeOffset UtcNow { get; }
    }
}
