using YouTubeV2.Application.DTO;

namespace YouTubeV2.Application.Model
{
    public class User
    {
        
        public Guid Id;
        public string Email;
        public string Nickname;
        public string Name;
        public string Surname;
        public string Password;

        public User() { }

        public User(string email, string nickname, string name, string surname, string password)
        {
            Email = email;
            Nickname = nickname;
            Name = name;
            Surname = surname;
            Password = password;
        }

        public User(RegisterDto registerDto)
        {
            Email = registerDto.email; 
            Nickname = registerDto.nickname;
            Name = registerDto.name;
            Surname = registerDto.surname;
            Password = registerDto.password;
        }
    }
}
