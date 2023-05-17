using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YouTubeV2.Application.Model;

namespace YouTubeV2.Application.EntityConfiguration
{
    internal class ReactionsEntityConfiguration : IEntityTypeConfiguration<Reaction>
    {
        public void Configure(EntityTypeBuilder<Reaction> builder)
        {
            builder
                .Metadata
                .FindNavigation(nameof(Reaction.Video))!
                .ForeignKey
                .DeleteBehavior = DeleteBehavior.Cascade;
        }
    }
}
