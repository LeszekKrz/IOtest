using FluentValidation;
using Microsoft.AspNetCore.Identity;
using YouTubeV2.Application.DTO;
using YouTubeV2.Application.Model;

namespace YouTubeV2.Application.Validator
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator(YTContext context, UserManager<User> userManager)
        {
            RuleFor(x => x.name).NotNull().Length(1, Constants.MaxUserNameLength);
            RuleFor(x => x.surname).NotNull().Length(1, Constants.MaxUserSurnameLength);

            RuleFor(x => x.nickname)
                .NotNull()
                .Length(1, Constants.MaxUserNicknameLength)
                .MustAsync(async (nickname, cancellationToken) => await userManager.FindByNameAsync(nickname.ToUpper()) == null)
                .WithMessage(x => $"User with nickname {x.nickname} already exists");

            RuleFor(x => x.email)
                .NotNull()
                .Length(1, Constants.MaxUserEmailLength)
                .EmailAddress()
                .MustAsync(async (email, cancellationToken) => await userManager.FindByEmailAsync(email.ToUpper()) == null)
                .WithMessage(x => $"User with email {x.email} already exists");

            RuleFor(x => x.password).NotEmpty().WithMessage("Your password cannot be empty")
                .MinimumLength(Constants.MinUserPasswordLength).WithMessage("Your password length must be at least 8.")
                .MaximumLength(Constants.MaxUserPasswordLength).WithMessage("Your password length must not exceed 16.")
                .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
                .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
                .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
                .Matches(@"[!@#$%^&*(),.<>?/]+").WithMessage("Your password must contain at least one special character.");
        }
    }
}
