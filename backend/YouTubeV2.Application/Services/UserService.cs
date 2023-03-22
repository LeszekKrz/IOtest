using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using YouTubeV2.Application.DTO;
using YouTubeV2.Application.Exceptions;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Services.AzureServices.BlobServices;
using YouTubeV2.Application.Validator;

namespace YouTubeV2.Application.Services
{
    public class UserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IBlobImageService _blobImageService;
        private readonly RegisterDtoValidator _registerDtoValidator;

        public UserService(UserManager<User> userManager, IBlobImageService blobImageService, RegisterDtoValidator registerDtoValidator)
        {
            _userManager = userManager;
            _blobImageService = blobImageService;
            _registerDtoValidator = registerDtoValidator;
        }

        public async Task RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken)
        {
            await _registerDtoValidator.ValidateAndThrowAsync(registerDto, cancellationToken);
            var user = new User(registerDto);

            var result = await _userManager.CreateAsync(user, registerDto.password);
            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.Select(error => new ErrorResponseDTO(error.Description)));

            if (registerDto.userType.ToUpper() == Role.Simple.ToUpper()) result = await _userManager.AddToRoleAsync(user, Role.Simple);
            else if (registerDto.userType.ToUpper() == Role.Creator.ToUpper()) result = await _userManager.AddToRoleAsync(user, Role.Creator);

            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.Select(error => new ErrorResponseDTO(error.Description)));

            if (registerDto.avatarImage.IsNullOrEmpty()) return;

            var newUser = await _userManager.FindByEmailAsync(registerDto.email);
            byte[] image = Convert.FromBase64String(registerDto.avatarImage);
            await _blobImageService.UploadProfilePictureAsync(image, user.Id, cancellationToken);
        }
    }
}
