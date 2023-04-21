using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using YouTubeV2.Application.DTO;
using YouTubeV2.Application.DTO.UserDTOS;
using YouTubeV2.Application.Exceptions;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Services.BlobServices;
using YouTubeV2.Application.Services.JwtFeatures;
using YouTubeV2.Application.Validator;

namespace YouTubeV2.Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IBlobImageService _blobImageService;
        private readonly RegisterDtoValidator _registerDtoValidator;
        private readonly LoginDtoValidator _loginDtoValidator;
        private readonly JwtHandler _jwtHandler;

        public UserService(UserManager<User> userManager, IBlobImageService blobImageService,
            RegisterDtoValidator registerDtoValidator, LoginDtoValidator loginDtoValidator, JwtHandler jwtHandler)
        {
            _userManager = userManager;
            _blobImageService = blobImageService;
            _registerDtoValidator = registerDtoValidator;
            _loginDtoValidator = loginDtoValidator;
            _jwtHandler = jwtHandler;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken)
        {
            await _loginDtoValidator.ValidateAndThrowAsync(loginDto, cancellationToken);

            User? user = await _userManager.FindByEmailAsync(loginDto.email)
                ?? throw new NotFoundException($"User with email {loginDto.email} does not exists");

            if (!await _userManager.CheckPasswordAsync(user, loginDto.password))
                throw new BadRequestException(new ErrorResponseDTO[] { new ErrorResponseDTO("Provided password is invalid") });

            return new LoginResponseDto($"Bearer {await _jwtHandler.GenerateTokenAsync(user)}");
        }

        public async Task RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken)
        {
            await _registerDtoValidator.ValidateAndThrowAsync(registerDto, cancellationToken);
            var user = new User(registerDto);

            var result = await _userManager.CreateAsync(user, registerDto.password);
            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.Select(error => new ErrorResponseDTO(error.Description)));

            if (registerDto.userType.Equals(Role.Simple, StringComparison.InvariantCultureIgnoreCase)) result = await _userManager.AddToRoleAsync(user, Role.Simple);
            else if (registerDto.userType.Equals(Role.Creator, StringComparison.InvariantCultureIgnoreCase))
            {
                result = await _userManager.AddToRoleAsync(user, Role.Simple);

                if (!result.Succeeded)
                    throw new BadRequestException(result.Errors.Select(error => new ErrorResponseDTO(error.Description)));

                result = await _userManager.AddToRoleAsync(user, Role.Creator);
            }


            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.Select(error => new ErrorResponseDTO(error.Description)));

            if (registerDto.avatarImage.IsNullOrEmpty()) return;

            var newUser = await _userManager.FindByEmailAsync(registerDto.email);
            await _blobImageService.UploadProfilePictureAsync(registerDto.avatarImage, user.Id, cancellationToken);
        }

        public async Task<User?> GetByIdAsync(string id) => await _userManager.FindByIdAsync(id);

        public ClaimsPrincipal? ValidateToken(string token) => _jwtHandler.ValidateToken(token);

        public static string GetTokenFromTokenWithBearerPrefix(string tokenWithBearerPrefix) => tokenWithBearerPrefix["Bearer ".Length..];
    }
}
