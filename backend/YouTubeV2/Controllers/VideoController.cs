using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YouTubeV2.Api.Attributes;
using YouTubeV2.Application.DTO.VideoMetadataDTOS;
using YouTubeV2.Application.Enums;
using YouTubeV2.Application.Jobs;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Services;
using YouTubeV2.Application.Services.BlobServices;
using YouTubeV2.Application.Services.VideoServices;

namespace YouTubeV2.Api.Controllers
{
    [ApiController]
    public class VideoController : ControllerBase
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
            if (claimsPrincipal == null) return Unauthorized();

            if (!claimsPrincipal.IsInRole(Role.Simple) && !claimsPrincipal.IsInRole(Role.Creator) && !claimsPrincipal.IsInRole(Role.Administrator))
                return Forbid();

            await _videoService.AuthorizeVideoAccessAsync(id, GetUserId(), cancellationToken);

            Stream videoStream = await _blobVideoService.GetVideoAsync(id.ToString(), cancellationToken);
            Response.Headers.AcceptRanges = "bytes";

            return File(videoStream, "video/mp4", true);
        }

        [HttpPost("video-metadata")]
        [Roles(Role.Creator)]
        public async Task<ActionResult<VideoMetadataPostResponseDto>> AddVideoMetadataAsync([FromBody] VideoMetadataPostDto videoMetadata, CancellationToken cancellationToken)
        {
            string userId = GetUserId();
            User? user = await _userService.GetByIdAsync(userId);
            if (user == null) return NotFound("There is no user identifiable by given token");

            Guid id = await _videoService.AddVideoMetadataAsync(videoMetadata, user, cancellationToken);
            return Ok(new VideoMetadataPostResponseDto(id.ToString()));
        }

        [HttpPost("video/{id:guid}")]
        [Roles(Role.Creator)]
        public async Task<ActionResult> UploadVideoAsync(Guid id, [FromForm] IFormFile videoFile, CancellationToken cancellationToken)
        {
            string videoExtension = Path.GetExtension(videoFile.FileName).ToLower();
            if (!_allowedVideoExtensions.Contains(videoExtension))
                return BadRequest($"Video extension provided ({videoExtension}) is not supported. Supported extensions: .mkv, .mp4, .avi, .webm");
            Video? video = await _videoService.GetVideoByIdAsync(id, cancellationToken, video => video.User);
            if (video == null)
                return NotFound($"Video with id {id} not found");
            if (video!.User.Id != GetUserId())
                return Forbid();
            if (video.ProcessingProgress != ProcessingProgress.MetadataRecordCreater && video.ProcessingProgress != ProcessingProgress.FailedToUpload)
                return BadRequest($"Trying to upload video which has processing progress {video.ProcessingProgress}");

            await _videoService.SetVideoProcessingProgressAsync(video, ProcessingProgress.Uploading, cancellationToken);
            await using var videoFileStream = videoFile.OpenReadStream();
            await _videoProcessingService.EnqueVideoProcessingJobAsync(new VideoProcessJob(id, videoFileStream, videoExtension));
;
            return StatusCode(StatusCodes.Status202Accepted);
        }

        [HttpGet("video-metadata")]
        [Roles(Role.Simple, Role.Creator, Role.Administrator)]
        public async Task<ActionResult<VideoMetadataDto>> GetVideoMetadataAsync([FromQuery]Guid id, CancellationToken cancellationToken)
        {
            await _videoService.AuthorizeVideoAccessAsync(id, GetUserId(), cancellationToken);

            return Ok(await _videoService.GetVideoMetadataAsync(id, cancellationToken));
        }

        private string GetUserId() => User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
    }
}
