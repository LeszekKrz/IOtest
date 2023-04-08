using FluentValidation;
using YouTubeV2.Application.DTO;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Providers;
using YouTubeV2.Application.Services.AzureServices.BlobServices;
using YouTubeV2.Application.Validator;

namespace YouTubeV2.Application.Services
{
    public class VideoService : IVideoService
    {
        private readonly YTContext _context;
        private readonly VideoMetadataPostValidator _videoMetadataDtoValidator;
        private readonly IBlobImageService _blobImageService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public VideoService(YTContext context ,VideoMetadataPostValidator videoMetadataDtoValidator, IBlobImageService blobImageService, IDateTimeProvider dateTimeProvider)
        {
            _context = context;
            _videoMetadataDtoValidator = videoMetadataDtoValidator;
            _blobImageService = blobImageService;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Guid> AddVideoMetadataAsync(VideoMetadataPostDTO videoMetadata, User user, CancellationToken cancellationToken = default)
        {
            await _videoMetadataDtoValidator.ValidateAndThrowAsync(videoMetadata, cancellationToken);
            var video = Video.FromDTO(videoMetadata, user, _dateTimeProvider.UtcNow);
            _context.Users.Attach(user);
            await _context.Videos.AddAsync(video, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            await _blobImageService.UploadVideoThumbnailAsync(videoMetadata.thumbnail, video.Id.ToString(), cancellationToken);
            return video.Id;
        }
    }
}
