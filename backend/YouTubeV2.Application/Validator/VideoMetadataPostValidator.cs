using FluentValidation;
using YouTubeV2.Application.Constants;
using YouTubeV2.Application.DTO.VideoMetadataDTOS;
using YouTubeV2.Application.Utils;

namespace YouTubeV2.Application.Validator
{
    public class VideoMetadataPostValidator : AbstractValidator<VideoMetadataPostDto>
    {
        public VideoMetadataPostValidator()
        {
            RuleFor(x => x.title).NotNull().Length(1, VideoConstants.titleMaxLength);
            RuleFor(x => x.description).NotNull().Length(1, VideoConstants.descriptionMaxLength);
            RuleFor(x => x.thumbnail)
                .Must(thumbnail => thumbnail.IsValidBase64Image())
                .WithMessage("Thumbnail must be a valid base64-encoded PNG or JPEG image.");
            RuleFor(x => x.tags).NotNull();
            RuleForEach(x => x.tags).Length(1, VideoConstants.tagMaxLength);
            RuleFor(x => x.visibility).NotNull().IsInEnum().WithMessage("Visibility must be either Private or Public.");
        }
    }
}
