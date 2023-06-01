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
using YouTubeV2.Application.Services.VideoServices;
using YouTubeV2.Application.Validator;

namespace YouTubeV2.Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IBlobImageService _blobImageService;
        private readonly RegisterDtoValidator _registerDtoValidator;
        private readonly UpdateUserDTOValidator _updateUserDTOValidator;
        private readonly LoginDtoValidator _loginDtoValidator;
        private readonly JwtHandler _jwtHandler;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IVideoService _videoService;

        public UserService(
            UserManager<User> userManager,
            IBlobImageService blobImageService,
            RegisterDtoValidator registerDtoValidator,
            LoginDtoValidator loginDtoValidator, 
            JwtHandler jwtHandler,
            ISubscriptionService subscriptionService,
            UpdateUserDTOValidator updateUserDTOValidator,
            IVideoService videoService)
        {
            _userManager = userManager;
            _blobImageService = blobImageService;
            _registerDtoValidator = registerDtoValidator;
            _loginDtoValidator = loginDtoValidator;
            _jwtHandler = jwtHandler;
            _subscriptionService = subscriptionService;
            _updateUserDTOValidator = updateUserDTOValidator;
            _videoService = videoService;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken)
        {
            await _loginDtoValidator.ValidateAndThrowAsync(loginDto, cancellationToken);

            User? user = await _userManager.FindByEmailAsync(loginDto.email)
                ?? throw new NotFoundException($"User with email {loginDto.email} does not exists");

            if (!await _userManager.CheckPasswordAsync(user, loginDto.password))
                throw new UnauthorizedException("Provided password is invalid");

            return new LoginResponseDto(await _jwtHandler.GenerateTokenAsync(user));
        }

        public async Task<string> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken)
        {
            await _registerDtoValidator.ValidateAndThrowAsync(registerDto, cancellationToken);
            var user = new User(registerDto);

            var result = await _userManager.CreateAsync(user, registerDto.password);
            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.Select(error => new ErrorResponseDTO(error.Description)));

            if (registerDto.userType.Equals(Role.Simple, StringComparison.InvariantCultureIgnoreCase)) result = await _userManager.AddToRoleAsync(user, Role.Simple);
            else if (registerDto.userType.Equals(Role.Creator, StringComparison.InvariantCultureIgnoreCase)) result = await _userManager.AddToRoleAsync(user, Role.Creator);

            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.Select(error => new ErrorResponseDTO(error.Description)));

            var newUser = await _userManager.FindByEmailAsync(registerDto.email);

            if (!registerDto.avatarImage.IsNullOrEmpty())
                await _blobImageService.UploadProfilePictureAsync(registerDto.avatarImage!, user.Id, cancellationToken);

            return newUser!.Id;
        }

        public async Task<UserDto> GetAsync(string callerID, string userID, CancellationToken cancellationToken)
        {
            var callingUser = await GetByIdAsync(callerID) ?? throw new UnauthorizedException();
            bool getAllData = false;

            User userToGet = callingUser;

            if (userID.IsNullOrEmpty())
            {
                getAllData = true;
            }
            else
            {
                if (await _userManager.IsInRoleAsync(callingUser!, Role.Administrator))
                    getAllData = true;

               userToGet = await GetByIdAsync(userID) ?? throw new NotFoundException("User not found");
            }

            return await GetDTOForUser(userToGet, getAllData, cancellationToken);
        }

        public async Task EditAsync(
            string callerID,
            string userQueryID,
            UpdateUserDto updateUserDTO, 
            CancellationToken cancellationToken)
        {
            var userToEdit = await VerifyAccessAndGetUserToModify(callerID, userQueryID);

            await _updateUserDTOValidator.ValidateAndThrowAsync(updateUserDTO, cancellationToken);
            await VerifyEmailUniqueAsync(userToEdit);
            await VerifyNameUniqueAsync(userToEdit);

            userToEdit.UserName = updateUserDTO.nickname;
            userToEdit.Name = updateUserDTO.name;
            userToEdit.Surname = updateUserDTO.surname;

            if (updateUserDTO.avatarImage.IsNullOrEmpty())
                await _blobImageService.DeleteProfilePictureAsync(userToEdit.Id, cancellationToken);
            else
                await _blobImageService.UploadProfilePictureAsync(updateUserDTO.avatarImage!, userToEdit.Id, cancellationToken);

            var currentRole = await GetRoleForUserAsync(userToEdit);
            var newRole = updateUserDTO.userType;

            if (currentRole.Equals(Role.Creator, StringComparison.InvariantCulture)
                && newRole.Equals(Role.Simple, StringComparison.InvariantCulture))
                await ProcessSwitchToSimple(userToEdit, cancellationToken);

            await SwitchUserRoleAsync(userToEdit, currentRole, newRole);
            
            await _userManager.UpdateAsync(userToEdit);
        }

        public async Task DeleteAsync(string callerID, string userID, CancellationToken cancellationToken)
        {
            var callingUser = await GetByIdAsync(callerID);

            if (!await _userManager.IsInRoleAsync(callingUser!, Role.Administrator))
                throw new ForbiddenException("Only ADMIN can delete users");

            var user = await GetByIdAsync(userID) ?? throw new BadRequestException("User not found");

            var userRoles = await _userManager.GetRolesAsync(user);
            var userClaims = await _userManager.GetClaimsAsync(user);

            foreach (var role in userRoles)
            {
                var roleResult = await _userManager.RemoveFromRoleAsync(user, role);
                if (!roleResult.Succeeded)
                    throw new BadRequestException(roleResult.Errors.Select(error => new ErrorResponseDTO(error.Description)));
            }

            foreach (var claim in userClaims)
            {
                var claimResult = await _userManager.RemoveClaimAsync(user, claim);
                if (!claimResult.Succeeded)
                    throw new BadRequestException(claimResult.Errors.Select(error => new ErrorResponseDTO(error.Description)));
            }

            await _subscriptionService.DeleteAllSubscriptionsRelatedToUserAsync(user, cancellationToken);

            await _blobImageService.DeleteProfilePictureAsync(user.Id, cancellationToken);

            var deleteResult = await _userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
                throw new BadRequestException(deleteResult.Errors.Select(error => new ErrorResponseDTO(error.Description)));
        }

        public async Task<User?> GetByIdAsync(string id) => await _userManager.FindByIdAsync(id);

        public ClaimsPrincipal? ValidateToken(string token) => _jwtHandler.ValidateToken(token);

        public async Task<UserDto> GetDTOForUser(User user, bool getAllData = false, CancellationToken cancellationToken = default)
        {
            var imageUri = _blobImageService.GetProfilePictureUrl(user.Id);
            var roleName = await GetRoleForUserAsync(user);
            decimal? accountBallance = getAllData ? user.AccountBalance : null;
            int? subscriptionsCount = await _userManager.IsInRoleAsync(user, Role.Creator) ?
                await _subscriptionService.GetSubscriptionCountAsync(user.Id, cancellationToken) : null;

            return new UserDto(new Guid(user.Id), user.Email!, user.UserName!, user.Name!, user.Surname!, accountBallance,
                roleName, imageUri.ToString(), subscriptionsCount);
        }

        private async Task<User> VerifyAccessAndGetUserToModify(string callerID, string userID)
        {
            var callingUser = await GetByIdAsync(callerID) ?? throw new UnauthorizedException();

            User userToModify = callingUser;

            if (await _userManager.IsInRoleAsync(callingUser, Role.Administrator))
            {
                userToModify = await GetByIdAsync(userID) ?? throw new BadRequestException("User not found");
            }
            else
            {
                if (!userID.IsNullOrEmpty() && userID != callerID)
                    throw new ForbiddenException("You have no permission to modify this user");
            }

            return userToModify;
        }

        private async Task<string> GetRoleForUserAsync(User user)
        { 
            var roles = await _userManager.GetRolesAsync(user);

            return roles.First();
        }

        private async Task VerifyEmailUniqueAsync(User user)
        {
            var userFound = await _userManager.FindByEmailAsync(user.Email!);
            if (userFound != null && userFound != user)
                throw new BadRequestException("User with this email already exists");
        }

        private async Task VerifyNameUniqueAsync(User user)
        {
            var userFound = await _userManager.FindByNameAsync(user.UserName!);
            if (userFound != null && userFound != user)
                throw new BadRequestException("User with this nickname already exists");
        }

        private async Task ProcessSwitchToSimple(User user, CancellationToken cancellationToken)
        {
            if (await _videoService.GetVideoCountAsync(user, cancellationToken) > 0)
                throw new BadRequestException("You cannot switch to a simple account type if you still have published videos");

            await _subscriptionService.DeleteAllSubscriptionsWhereUserIsSubscribeeAsync(user, cancellationToken);
        }

        private async Task SwitchUserRoleAsync(User user, string currentRole, string newRole)
        {
            var result = await _userManager.RemoveFromRoleAsync(user, currentRole);
            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.Select(error => new ErrorResponseDTO(error.Description)));

            result = await _userManager.AddToRoleAsync(user, newRole);
            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.Select(error => new ErrorResponseDTO(error.Description)));
        }
    }
}
