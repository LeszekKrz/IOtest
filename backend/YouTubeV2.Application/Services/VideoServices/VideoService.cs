using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using YouTubeV2.Api.Enums;
using YouTubeV2.Application.DTO;
using YouTubeV2.Application.Enums;
using YouTubeV2.Application.Exceptions;
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

        public async Task<VideoMetadataDto> GetVideoMetadataAsync(Guid id, CancellationToken cancellationToken = default)
        {
            Video video;
            Uri thumbnail;

            try
            {
                video = await _context.Videos
                    .Include(video => video.Tags)
                    .Include(video => video.User)
                    .SingleAsync(video => video.Id == id, cancellationToken);
                thumbnail = _blobImageService.GetVideoThumbnail(video.Id.ToString());
            }
            catch (Exception ex)
            {
                throw new NotFoundException("Video not found");
            }

            video.ViewCount++;
            await _context.SaveChangesAsync(cancellationToken);

            return new VideoMetadataDto(
                video.Id.ToString(),
                video.Title,
                video.Description,
                thumbnail.ToString(),
                video.User.Id,
                video.User.UserName,
                video.ViewCount,
                video.Tags.Select(tag => tag.ToString()).ToList(),
                video.Visibility.ToString(),
                video.ProcessingProgress.ToString(),
                video.UploadDate.DateTime,
                video.EditDate.DateTime,
                video.Duration);
        }

        public async Task AuthorizeVideoAccessAsync(Guid videoId, string userId, CancellationToken cancellationToken = default)
        {
            var video = await _context.Videos
                    .Include(video => video.User)
                    .SingleAsync(video => video.Id == videoId, cancellationToken);

            if (video.Visibility == Visibility.Private && video.User.Id != userId)
                throw new ForbiddenException("Video is private");
        }

        public async Task SetVideoLengthAsync(Video video, double length, CancellationToken cancellationToken = default)
        {
            TimeSpan time = TimeSpan.FromSeconds(length);
            string formattedTime = time.ToString(@"hh\:mm\:ss");
            var vid = await _context.Videos.SingleAsync(vid => vid.Id == video.Id, cancellationToken);
            vid.Duration = formattedTime;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
