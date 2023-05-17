using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using YouTubeV2.Api.Enums;
using YouTubeV2.Application.DTO.VideoDTOS;
using YouTubeV2.Application.DTO.VideoMetadataDTOS;
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

        public async Task<Guid> AddVideoMetadataAsync(VideoMetadataAddOrUpdateDto videoMetadata, User user, CancellationToken cancellationToken = default)
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
                    .Include(video => video.Author)
                    .SingleAsync(video => video.Id == id, cancellationToken);
                thumbnail = _blobImageService.GetVideoThumbnailUrl(video.Id.ToString());
            }
            catch
            {
                throw new NotFoundException("Video not found");
            }

            video.ViewCount++;
            await _context.SaveChangesAsync(cancellationToken);

            return new VideoMetadataDto(
                video.Id,
                video.Title,
                video.Description,
                thumbnail,
                video.Author.Id,
                video.Author.UserName!,
                video.ViewCount,
                video.Tags.Select(tag => tag.Value).ToList(),
                video.Visibility,
                video.ProcessingProgress,
                video.UploadDate.DateTime,
                video.EditDate.DateTime,
                video.Duration);
        }

        public async Task AuthorizeVideoAccessAsync(Guid videoId, string userId, CancellationToken cancellationToken = default)
        {
            var video = await _context.Videos
                    .Include(video => video.Author)
                    .SingleAsync(video => video.Id == videoId, cancellationToken);

            if (video.Visibility == Visibility.Private && video.Author.Id != userId)
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

        public async Task<VideoListDto> GetAllUserVideos(string userId, CancellationToken cancellationToken = default)
        {
            List<VideoMetadataDto> videos = await _context
                .Videos
                .Include(video => video.Author)
                .Include(video => video.Tags)
                .Where(video => video.Author.Id == userId)
                .OrderByDescending(video => video.UploadDate)
                .ToVideoMetadataDto(_blobImageService)
                .ToListAsync(cancellationToken);

            return new VideoListDto(videos);
        }

        public async Task<VideoListDto> GetAllAvailableUserVideos(string userId, CancellationToken cancellationToken = default)
        {
            List<VideoMetadataDto> videos = await _context
                .Videos
                .Include(video => video.Author)
                .Include(video => video.Tags)
                .Where(video => video.Author.Id == userId
                    && video.Visibility == Visibility.Public
                    && video.ProcessingProgress == ProcessingProgress.Ready)
                .OrderByDescending(video => video.UploadDate)
                .ToVideoMetadataDto(_blobImageService)
                .ToListAsync(cancellationToken);

            return new VideoListDto(videos);
        }

        public async Task<VideoListDto> GetVideosFromSubscriptionsAsync(string userId, CancellationToken cancellationToken = default)
        {
            List<VideoMetadataDto> videos = await _context
                .Subscriptions
                .Include(subscription => subscription.Subscribee)
                .ThenInclude(subscribee => subscribee.Videos)
                .ThenInclude(video => video.Tags)
                .Where(subscription => subscription.SubscriberId == userId)
                .SelectMany(subscription => subscription.Subscribee.Videos)
                .Where(video => video.Visibility == Visibility.Public && video.ProcessingProgress == ProcessingProgress.Ready)
                .OrderByDescending(video => video.UploadDate)
                .ToVideoMetadataDto(_blobImageService)
                .ToListAsync(cancellationToken);

            return new VideoListDto(videos);
        }

        public async Task DeleteVideoAsync(Video video, CancellationToken cancellationToken = default)
        {
            _context.Videos.Remove(video);
            await _context.SaveChangesAsync(cancellationToken);

            await _blobImageService.DeleteThumbnailAsync(video.Id.ToString(), cancellationToken);
        }

        public async Task<int> GetVideoCountAsync(User user, CancellationToken cancellationToken = default) => 
            await _context.Videos.CountAsync(video => video.Author == user, cancellationToken);

        public async Task UpdateVideoMetadataAsync(VideoMetadataAddOrUpdateDto videoMetadata, Video video, CancellationToken cancellationToken = default)
        {
            await _videoMetadataDtoValidator.ValidateAndThrowAsync(videoMetadata, cancellationToken);

            video.Title = videoMetadata.title;
            video.Description = videoMetadata.description;
            video.Visibility = videoMetadata.visibility;
            _context.Tags.RemoveRange(video.Tags);
            await _context.Tags.AddRangeAsync(videoMetadata.tags.Select(tag => new Tag(tag, video)));
            await _context.SaveChangesAsync(cancellationToken);

            await _blobImageService.UploadVideoThumbnailAsync(videoMetadata.thumbnail, video.Id.ToString(), cancellationToken);
        }
    }
}
