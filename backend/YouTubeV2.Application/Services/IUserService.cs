using System.Security.Claims;
using YouTubeV2.Application.DTO.UserDTOS;
using YouTubeV2.Application.Model;

namespace YouTubeV2.Application.Services
{
    public interface IUserService
    {
        Task<string> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken);

        Task<LoginResponseDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken);

        Task<UserDto> GetAsync(string callerID, string userID, CancellationToken cancellationToken);

        Task EditAsync(string callerID, string userQueryID, UpdateUserDto updateUserDTO, CancellationToken cancellationToken);

        Task DeleteAsync(string callerID, string userID, CancellationToken cancellationToken);

        Task<UserDto> GetDTOForUser(User user, bool getAllData, CancellationToken cancellationToken = default);

        Task<User?> GetByIdAsync(string id);

        ClaimsPrincipal? ValidateToken(string token);
    }
}
