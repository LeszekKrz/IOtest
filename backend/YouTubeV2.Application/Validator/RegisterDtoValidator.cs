using FluentValidation;
using YouTubeV2.Application.DTO;

namespace YouTubeV2.Application.Validator
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.email).NotEmpty();
            RuleFor(x => x.password).NotEmpty();
        }
    }
}
