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
               .NotNull();

            RuleFor(x => x.password)
                .NotNull();
        }
    }
}