using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YouTubeV2.Application.DTO;
using YouTubeV2.Application.Model;

namespace YouTubeV2.Application.Validator
{
    internal class SubmitTicketValidator : AbstractValidator<SubmitTicketDTO>
    {
        public SubmitTicketValidator(UserManager<User> userManager, YTContext context)
        {
            RuleFor(x => x.reason)
                .NotEmpty()
                .WithMessage("Reason cannot be empty");

            RuleFor(x => x.targetId)
                .MustAsync(async (targetId, cancellationToken) =>
                {
                    var user = await userManager.FindByIdAsync(targetId.ToString());
                    if (user is not null) return true;

                    var video = await context.Videos.SingleOrDefaultAsync(x => x.Id == targetId, cancellationToken);
                    if (video is not null) return true;

                    return false;
                });
        }
    }
}
