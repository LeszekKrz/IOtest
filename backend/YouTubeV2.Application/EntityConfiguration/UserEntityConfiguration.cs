using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YouTubeV2.Application.Model;

namespace YouTubeV2.Application.EntityConfiguration
{
    public class UserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.NormalizedEmail).IsUnique();
            builder.HasIndex(x => x.NormalizedUserName).IsUnique();

            builder.Property(x => x.Name);
            builder.Property(x => x.Surname);

            builder.HasMany(x => x.UserRoles)
                .WithOne(x => x.User)
                .HasForeignKey(userRole => userRole.UserId)
                .IsRequired();
        }
    }
}
