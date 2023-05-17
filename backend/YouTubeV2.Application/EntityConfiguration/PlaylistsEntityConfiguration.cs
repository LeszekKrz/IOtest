using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YouTubeV2.Application.Model;

namespace YouTubeV2.Application.EntityConfiguration
{
    internal class PlaylistsEntityConfiguration : IEntityTypeConfiguration<Playlist>
    {
        public void Configure(EntityTypeBuilder<Playlist> builder)
        {
            builder
                .HasMany(p => p.Videos)
                .WithMany(v => v.Playlists)
                .UsingEntity<Dictionary<string, object>>(
                    "PlaylistVideo",
                    playlistVideo => playlistVideo
                        .HasOne<Video>()
                        .WithMany()
                        .HasForeignKey("VideoId")
                        .OnDelete(DeleteBehavior.Cascade),
                    playlistVideo => playlistVideo
                        .HasOne<Playlist>()
                        .WithMany()
                        .HasForeignKey("PlaylistId"));
        }
    }
}
