using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YouTubeV2.Application.Model;

namespace YouTubeV2.Application.EntityConfiguration
{
    internal class SubscriptionEntityConfiguration : IEntityTypeConfiguration<Subscription>
    {
        public void Configure(EntityTypeBuilder<Subscription> builder)
        {
            builder.HasKey(s => new { s.SubscriberId, s.SubscribeeId });
            builder.HasOne(s => s.Subscriber).WithMany().HasForeignKey(s => s.SubscriberId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(s => s.Subscribee).WithMany(s => s.Subscriptions).HasForeignKey(s => s.SubscribeeId);
        }
    }
}