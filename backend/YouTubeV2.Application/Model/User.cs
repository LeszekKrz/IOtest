using Microsoft.AspNetCore.Identity;
using YouTubeV2.Application.DTO;

namespace YouTubeV2.Application.Model
{
    public class User : IdentityUser
    {
        public string Name;
        public string Surname;

        public ICollection<UserRole> UserRoles;

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
