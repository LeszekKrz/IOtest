using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using YouTubeV2.Api.Attributes;
using YouTubeV2.Application.DTO.VideoDTOS;
using YouTubeV2.Application.DTO.VideoMetadataDTOS;
using YouTubeV2.Application.Enums;
using YouTubeV2.Application.Jobs;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Services;
using YouTubeV2.Application.Services.BlobServices;
using YouTubeV2.Application.Services.JwtFeatures;
using YouTubeV2.Application.Services.VideoServices;

namespace YouTubeV2.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class VideoController : IdentityControllerBase
    {
        private readonly IBlobVideoService _blobVideoService;
        private readonly IVideoService _videoService;
        private readonly IUserService _userService;
        private readonly IVideoProcessingService _videoProcessingService;
        private readonly IReadOnlyCollection<string> _allowedVideoExtensions = new string[] { ".mkv", ".mp4", ".avi", ".webm" };

        public VideoController(
            IBlobVideoService blobVideoService,
            IVideoService videoService,
            IUserService userService,
            IVideoProcessingService videoProcessingService)
        {
            _blobVideoService = blobVideoService;
            _videoService = videoService;
            _userService = userService;
            _videoProcessingService = videoProcessingService;
        }

        [HttpGet("video/{id:guid}")]
        public async Task<IActionResult> GetVideoAsync(Guid id, [FromQuery] string access_token, CancellationToken cancellationToken)
        {
            ClaimsPrincipal? claimsPrincipal = _userService.ValidateToken(access_token);
            if (claimsPrincipal is null) return Unauthorized();

            if (!claimsPrincipal.IsInRole(Role.Simple) && !claimsPrincipal.IsInRole(Role.Creator) && !claimsPrincipal.IsInRole(Role.Administrator))
                return Forbid();

            await _videoService.AuthorizeVideoAccessAsync(id, claimsPrincipal.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value, cancellationToken);

            Stream videoStream = await _blobVideoService.GetVideoAsync(id.ToString(), cancellationToken);
            Response.Headers.AcceptRanges = "bytes";

            return File(videoStream, "video/mp4", true);
        }

        [HttpPost("video-metadata")]
        [Roles(Role.Creator)]
        public async Task<ActionResult<VideoMetadataPostResponseDto>> AddVideoMetadataAsync([FromBody] VideoMetadataAddOrUpdateDto videoMetadata, CancellationToken cancellationToken)
        {
            string? userId = GetUserId();
            if (userId is null) return Forbid();

            User? user = await _userService.GetByIdAsync(userId);
            if (user is null) return NotFound("There is no user identifiable by given token");

            Guid id = await _videoService.AddVideoMetadataAsync(videoMetadata, user, cancellationToken);
            return Ok(new VideoMetadataPostResponseDto(id.ToString()));
        }

        [HttpPut("video-metadata")]
        [Roles(Role.Creator)]
        public async Task<ActionResult> UpdateVideoMetadataAsync([FromQuery][Required] Guid id, [FromBody] VideoMetadataAddOrUpdateDto videoMetadata, CancellationToken cancellationToken)
        {
            string? userId = GetUserId();
            if (userId is null) return Forbid();

            Video? video = await _videoService.GetVideoByIdAsync(id, cancellationToken, video => video.Author, video => video.Tags);
            if (video is null) return NotFound($"Video with id {id} not found");

            if (userId != video.Author.Id) return Forbid("You are not the owner of the video");

            await _videoService.UpdateVideoMetadataAsync(videoMetadata, video, cancellationToken);

            return Ok();
        }

        [HttpPost("video/{id:guid}")]
        [Roles(Role.Creator)]
        public async Task<ActionResult> UploadVideoAsync(Guid id, [FromForm] IFormFile videoFile, CancellationToken cancellationToken)
        {
            string videoExtension = Path.GetExtension(videoFile.FileName).ToLower();
            if (!_allowedVideoExtensions.Contains(videoExtension))
                return BadRequest($"Video extension provided ({videoExtension}) is not supported. Supported extensions: .mkv, .mp4, .avi, .webm");
            Video? video = await _videoService.GetVideoByIdAsync(id, cancellationToken, video => video.Author);
            if (video is null)
                return NotFound($"Video with id {id} not found");
            if (video!.Author.Id != GetUserId())
                return Forbid();
            if (video.ProcessingProgress != ProcessingProgress.MetadataRecordCreater && video.ProcessingProgress != ProcessingProgress.FailedToUpload)
                return BadRequest($"Trying to upload video which has processing progress {video.ProcessingProgress}");

            await _videoService.SetVideoProcessingProgressAsync(video, ProcessingProgress.Uploading, cancellationToken);
            MemoryStream videoFileStream = new();
            await videoFile.CopyToAsync(videoFileStream, cancellationToken);
            videoFileStream.Seek(0, SeekOrigin.Begin);
            await _videoProcessingService.EnqueVideoProcessingJobAsync(new VideoProcessJob(id, videoFileStream, videoExtension));
;
            return StatusCode(StatusCodes.Status202Accepted);
        }

        [HttpGet("video-metadata")]
        [Roles(Role.Simple, Role.Creator, Role.Administrator)]
        public async Task<ActionResult<VideoMetadataDto>> GetVideoMetadataAsync([FromQuery]Guid id, CancellationToken cancellationToken)
        {
            string? userId = GetUserId();
            if (userId is null) return Forbid();

            await _videoService.AuthorizeVideoAccessAsync(id, userId, cancellationToken);

            return Ok(await _videoService.GetVideoMetadataAsync(id, cancellationToken));
        }

        [HttpGet("user/videos")]
        [Roles(Role.Simple, Role.Creator, Role.Administrator)]
        public async Task<ActionResult<VideoListDto>> GetUserVideosAsync([FromQuery] string? id, CancellationToken cancellationToken)
        {
            string? userId = GetUserId();
            if (userId is null) return Forbid();

            return userId == id || id is null
                ? await _videoService.GetAllUserVideos(userId, cancellationToken)
                : await _videoService.GetAllAvailableUserVideos(id, cancellationToken);
        }

        [HttpGet("user/videos/subscribed")]
        [Roles(Role.Simple, Role.Creator, Role.Administrator)]
        public async Task<ActionResult<VideoListDto>> GetVideosFromSubscriptionsAsync(CancellationToken cancellationToken)
        {
            string? userId = GetUserId();
            if (userId is null) return Forbid();

            return await _videoService.GetVideosFromSubscriptionsAsync(userId, cancellationToken);
        }

        [HttpDelete("video")]
        [Roles(Role.Creator, Role.Administrator)]
        public async Task<ActionResult> DeleteVideoAsync([FromQuery][Required] Guid id, CancellationToken cancellationToken)
        {
            Video? video = await _videoService.GetVideoByIdAsync(id, cancellationToken, video => video.Author);
            if (video is null) return NotFound();

            string? userId = GetUserId();
            string? role = GetUserRole();
            if (userId is null || role is null) return Forbid();

            if (video.Author.Id != userId && role != Role.Administrator) return Forbid();

            await _videoService.DeleteVideoAsync(video, cancellationToken);
            return Ok();
        }
    }
}
