using YouTubeV2.Api.Enums;

namespace YouTubeV2.Application.Model
{
    public class Playlist
    {
        public Guid Id { get; init; }
        public User Creator { get; init; } = null!;
        public string Name { get; set; } = null!;
        public Visibility Visibility { get; set; }
        public virtual ICollection<Video> Videos { get; set; } = null!;
    }
}
