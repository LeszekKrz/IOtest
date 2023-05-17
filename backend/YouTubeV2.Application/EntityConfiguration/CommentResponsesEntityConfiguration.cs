using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YouTubeV2.Application.Model;

namespace YouTubeV2.Application.EntityConfiguration
{
    internal class CommentResponsesEntityConfiguration : IEntityTypeConfiguration<CommentResponse>
    {
        public void Configure(EntityTypeBuilder<CommentResponse> builder)
        {
            builder
                .Metadata
                .FindNavigation(nameof(CommentResponse.RespondOn))!
                .ForeignKey
                .DeleteBehavior = DeleteBehavior.Cascade;
        }
    }
}
