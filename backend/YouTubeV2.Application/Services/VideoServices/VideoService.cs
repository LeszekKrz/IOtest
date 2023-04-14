using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using YouTubeV2.Application.DTO;
using YouTubeV2.Application.Enums;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Providers;
using YouTubeV2.Application.Services.BlobServices;
using YouTubeV2.Application.Validator;

namespace YouTubeV2.Application.Services.VideoServices
{
    public class VideoService : IVideoService
    {
        private readonly YTContext _context;
        private readonly VideoMetadataPostValidator _videoMetadataDtoValidator;
        private readonly IBlobImageService _blobImageService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public VideoService(YTContext context, VideoMetadataPostValidator videoMetadataDtoValidator, IBlobImageService blobImageService, IDateTimeProvider dateTimeProvider)
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

        public async Task<Video?> GetVideoByIdAsync(Guid id, CancellationToken cancellationToken = default, params Expression<Func<Video, object>>[] includes)
        {
            var query = _context.Videos.AsTracking();
            query = includes.Aggregate(query, (current, includeExpresion) => current.Include(includeExpresion));
            return await query.FirstOrDefaultAsync(video => video.Id == id, cancellationToken);
        }

        public async Task SetVideoProcessingProgressAsync(Video video, ProcessingProgress processingProgress, CancellationToken cancellationToken = default)
        {
            video.ProcessingProgress = processingProgress;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
