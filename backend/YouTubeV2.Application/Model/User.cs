using Microsoft.AspNetCore.Identity;
using YouTubeV2.Application.DTO.UserDTOS;

namespace YouTubeV2.Application.Model
{
    public class User : IdentityUser
    {
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public decimal AccountBalance { get; set; } = decimal.Zero;
        public DateTimeOffset CreationDate { get; init; }
        public virtual ICollection<Subscription> Subscriptions { get; init; } = null!;
        public virtual IReadOnlyCollection<Video> Videos { get; init; } = null!;
        public virtual ICollection<Playlist> Playlists { get; init; } = null!;

        public User() { }

        public User(RegisterDto registerDto, DateTimeOffset creationDate)
        {
            Email = registerDto.email; 
            UserName = registerDto.nickname;
            Name = registerDto.name;
            Surname = registerDto.surname;
            CreationDate = creationDate;
        }

        public User(RegisterDto registerDto)
        {
            Email = registerDto.email;
            UserName = registerDto.nickname;
            Name = registerDto.name;
            Surname = registerDto.surname;
        }

        public User(string email, string nickname, string name, string surname)
        {
            Email = email;
            UserName = nickname;
            Name = name;
            Surname = surname;
        }
    }
}
