using YouTubeV2.Application.DTO.VideoMetadataDTOS;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Services.BlobServices;

namespace YouTubeV2.Application.Services.VideoServices
{
    internal static class VideoExtensions
    {
        internal static IQueryable<VideoMetadataDto> ToVideoMetadataDto(this IQueryable<Video> query, IBlobImageService blobImageService) =>
            query.Select(video => new VideoMetadataDto(
                video.Id,
                video.Title,
                video.Description,
                blobImageService.GetVideoThumbnailUrl(video.Id.ToString()),
                video.Author.Id,
                video.Author.UserName!,
                video.ViewCount,
                video.Tags.Select(tag => tag.Value).ToList(),
                video.Visibility,
                video.ProcessingProgress,
                video.UploadDate.ToUniversalTime().Date,
                video.EditDate.ToUniversalTime().Date,
                video.Duration));
    }
}
