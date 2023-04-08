using YouTubeV2.Application.DTO;
using YouTubeV2.Application.Model;

namespace YouTubeV2.Application.Services
{
    public interface IUserService
    {
        public Task<LoginResponseDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken);

        public Task RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken);

        public Task<User?> GetByIdAsync(string id);
    }
}
