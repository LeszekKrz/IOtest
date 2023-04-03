using FluentValidation;
using Microsoft.AspNetCore.Identity;
using YouTubeV2.Application.Constants;
using YouTubeV2.Application.DTO;
using YouTubeV2.Application.Model;

namespace YouTubeV2.Application.Validator
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator(UserManager<User> userManager)
        {
            RuleFor(x => x.email)
              .NotNull()
              .Length(1, UserConstants.MaxUserEmailLength)
              .EmailAddress()
              .MustAsync(async (email, cancellationToken) => await userManager.FindByEmailAsync(email) != null)
              .WithMessage(x => $"User with email {x.email} does not exist");

            RuleFor(x => x.password)
                .NotNull();
        }
    }
}