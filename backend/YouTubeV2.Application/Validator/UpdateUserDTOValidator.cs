using FluentValidation;
using YouTubeV2.Application.Constants;
using YouTubeV2.Application.DTO.UserDTOS;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Utils;

namespace YouTubeV2.Application.Validator
{
    public class UpdateUserDTOValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserDTOValidator()
        {
            RuleFor(x => x.name).NotNull().Length(1, UserConstants.MaxUserNameLength);
            RuleFor(x => x.surname).NotNull().Length(1, UserConstants.MaxUserSurnameLength);
            RuleFor(x => x.userType).Must(userType => userType.Equals(Role.Simple, StringComparison.InvariantCultureIgnoreCase)
                || userType.Equals(Role.Creator, StringComparison.InvariantCultureIgnoreCase))
                .WithMessage($"User type has to be either {Role.Simple} or {Role.Creator}");

            RuleFor(x => x.nickname)
                .NotNull()
                .Length(1, UserConstants.MaxUserNicknameLength);

            RuleFor(x => x.avatarImage)
                .Must(avatarImage => avatarImage.IsValidBase64ImageOrNullOrEmpty())
                .WithMessage("Avatar must be a valid base64-encoded PNG or JPEG image or empty.");
        }
    }
}
