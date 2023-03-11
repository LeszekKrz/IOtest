using Microsoft.AspNetCore.Identity;
using YouTubeV2.Application.DTO;
using YouTubeV2.Application.Exceptions;
using YouTubeV2.Application.Model;

namespace YouTubeV2.Application.Services
{
    public class UserService
    {
        private readonly YTContext _context;
        private readonly UserManager<User> _userManager;

        public UserService(YTContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken)
        {
            var user = new User(registerDto);

            var result = await _userManager.CreateAsync(user, registerDto.password);
            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.Select(error => new ErrorResponseDTO(error.Description)));

            result = await _userManager.AddToRoleAsync(user, Role.User);
            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.Select(error => new ErrorResponseDTO(error.Description)));
        }
    }
}
