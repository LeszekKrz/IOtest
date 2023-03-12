using Microsoft.AspNetCore.Identity;
using YouTubeV2.Application.DTO;

namespace YouTubeV2.Application.Model
{
    public class User : IdentityUser
    {
        public string Name { get; init; }
        public string Surname { get; init; }

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
