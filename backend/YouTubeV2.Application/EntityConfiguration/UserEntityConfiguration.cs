using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Constants;

namespace YouTubeV2.Application.EntityConfiguration
{
    public class UserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(UserConstants.MaxUserNameLength);
            builder.Property(x => x.Surname).HasMaxLength(UserConstants.MaxUserSurnameLength);
            builder.Property(x => x.Email).HasMaxLength(UserConstants.MaxUserEmailLength);
            builder.Property(x => x.UserName).HasMaxLength(UserConstants.MaxUserNicknameLength);
        }
    }
}
