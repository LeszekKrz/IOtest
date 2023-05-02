using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YouTubeV2.Application.Model;

namespace YouTubeV2.Application.EntityConfiguration
{
    internal class VideosEntityConfigurations : IEntityTypeConfiguration<Video>
    {
        public void Configure(EntityTypeBuilder<Video> builder)
        {
            builder
                .Metadata
                .FindNavigation(nameof(Comment.Author))!
                .ForeignKey
                .DeleteBehavior = DeleteBehavior.Cascade;
        }
    }
}
