using Microsoft.AspNetCore.Identity;
using YouTubeV2.Application.DTO.UserDTOS;

namespace YouTubeV2.Application.Model
{
    public class User : IdentityUser
    {
        public string Name { get; init; } = null!;
        public string Surname { get; init; } = null!;
        public virtual ICollection<Subscription> Subscriptions { get; init; } = null!;
        public virtual IReadOnlyCollection<Video> Videos { get; init; } = null!;

        public User() { }

        public User(RegisterDto registerDto)
        {
            Email = registerDto.email; 
            UserName = registerDto.nickname;
            Name = registerDto.name;
            Surname = registerDto.surname;
        }
    }
}
