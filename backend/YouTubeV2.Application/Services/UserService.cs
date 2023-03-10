using YouTubeV2.Application.DTO;
using YouTubeV2.Application.Model;

namespace YouTubeV2.Application.Services
{
    public class UserService
    {
        private readonly YTContext _context;

        public UserService(YTContext context)
        {
            _context = context;
        }

        public async Task Register(RegisterDto registerDto)
        {
            var user = new User(registerDto);

            await _context.Users.AddAsync(user);

            await _context.SaveChangesAsync();
        }
    }
}
