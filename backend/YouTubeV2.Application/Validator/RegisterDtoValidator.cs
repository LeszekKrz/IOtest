using FluentValidation;
using Microsoft.AspNetCore.Identity;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Constants;
using YouTubeV2.Application.Utils;
using YouTubeV2.Application.DTO.UserDTOS;

namespace YouTubeV2.Application.Validator
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator(UserManager<User> userManager)
        {
            RuleFor(x => x.name).NotNull().Length(1, UserConstants.MaxUserNameLength);
            RuleFor(x => x.surname).NotNull().Length(1, UserConstants.MaxUserSurnameLength);
            RuleFor(x => x.userType).Must(userType => userType.Equals(Role.Simple, StringComparison.InvariantCultureIgnoreCase) 
                || userType.Equals(Role.Creator, StringComparison.InvariantCultureIgnoreCase));

            RuleFor(x => x.nickname)
                .NotNull()
                .Length(1, UserConstants.MaxUserNicknameLength)
                .MustAsync(async (nickname, cancellationToken) => await userManager.FindByNameAsync(nickname) == null)
                .WithMessage(x => $"User with nickname {x.nickname} already exists");

            RuleFor(x => x.email)
                .NotNull()
                .Length(1, UserConstants.MaxUserEmailLength)
                .EmailAddress()
                .MustAsync(async (email, cancellationToken) => await userManager.FindByEmailAsync(email) == null)
                .WithMessage(x => $"User with email {x.email} already exists");

            RuleFor(x => x.password)
                .NotNull()
                .MinimumLength(UserConstants.MinUserPasswordLength).WithMessage($"Your password length must be at least {UserConstants.MinUserPasswordLength}.")
                .MaximumLength(UserConstants.MaxUserPasswordLength).WithMessage($"Your password length must not exceed {UserConstants.MaxUserPasswordLength}.")
                .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
                .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
                .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
                .Matches(@"[!@#$%^&*(),.<>?/]+").WithMessage("Your password must contain at least one special character.");

            RuleFor(x => x.avatarImage)
                .Must(avatarImage => avatarImage.IsValidBase64ImageOrEmpty())
                .WithMessage("Avatar must be a valid base64-encoded PNG or JPEG image or empty.");
        }
    }
}
