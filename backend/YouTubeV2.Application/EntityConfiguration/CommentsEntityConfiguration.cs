using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YouTubeV2.Application.Model;

namespace YouTubeV2.Application.EntityConfiguration
{
    internal class CommentsEntityConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder
                .Metadata
                .FindNavigation(nameof(Comment.Video))!
                .ForeignKey
                .DeleteBehavior = DeleteBehavior.Cascade;
        }
    }
}
