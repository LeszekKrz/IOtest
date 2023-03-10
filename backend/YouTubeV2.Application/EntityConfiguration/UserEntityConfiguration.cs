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
            builder.Property(x => x.Id).HasDefaultValueSql("NEWID()");
            
            builder.Property(x => x.Email);
            builder.Property(x => x.Nickname);
            builder.Property(x => x.Name);
            builder.Property(x => x.Surname);
            builder.Property(x => x.Password);
        }
    }
}
