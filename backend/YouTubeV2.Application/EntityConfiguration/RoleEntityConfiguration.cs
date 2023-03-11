using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YouTubeV2.Application.Model;

namespace YouTubeV2.Application.EntityConfiguration
{
    internal class RoleEntityConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasMany(x => x.UsersRoles)
                .WithOne(x => x.Role)
                .HasForeignKey(userRole => userRole.RoleId)
                .IsRequired();

            builder.HasData(
                new Role(Role.User),
                new Role(Role.Admin));
        }
    }
}
