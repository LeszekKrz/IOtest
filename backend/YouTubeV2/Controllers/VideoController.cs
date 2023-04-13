using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YouTubeV2.Api.Attributes;
using YouTubeV2.Application.Constants;
using YouTubeV2.Application.DTO;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Services;
using YouTubeV2.Application.Services.AzureServices.BlobServices;

namespace YouTubeV2.Api.Controllers
{
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly IBlobVideoService _blobVideoService;
        private readonly IVideoService _videoService;
        private readonly IUserService _userService;

        public VideoController(IBlobVideoService blobVideoService, IVideoService videoService, IUserService userService)
        {
            _blobVideoService = blobVideoService;
            _videoService = videoService;
            _userService = userService;
        }

        [HttpGet("video/{id:guid}")]
        [Roles(Role.Simple, Role.Creator, Role.Administrator)]
        public async Task<IActionResult> GetVideoAsync(Guid id, CancellationToken cancellationToken)
        {
            // + ".mp4" is temporary as adding files from local file system seems to be adding extensions as prefix to the name (will change with uploading video from our portal)
            Stream videoStream = await _blobVideoService.GetVideoAsync(id.ToString() + ".mp4", cancellationToken);
            Response.Headers.AcceptRanges = "bytes";

            return File(videoStream, "video/mp4", true);
        }

        [HttpPost("video-metadata")]
        [Roles(Role.Creator)]
        public async Task<ActionResult<VideoMetadataPostResponseDTO>> AddVideoMetadataAsync([FromBody] VideoMetadataPostDTO videoMetadata, CancellationToken cancellationToken)
        {
            string userId = GetUserId();
            User? user = await _userService.GetByIdAsync(userId);
            if (user == null) return NotFound("There is no user identifiable by given token");

            Guid id = await _videoService.AddVideoMetadataAsync(videoMetadata, user, cancellationToken);
            return Ok(new VideoMetadataPostResponseDTO(id.ToString()));
        }

        [HttpGet("video-metadata")]
        //[Roles(Role.Simple, Role.Creator, Role.Administrator)]
        public async Task<VideoMetadataDto> GetVideoMetadataAsync([FromQuery]Guid id, CancellationToken cancellationToken)
        {
            return await _videoService.GetVideoMetadataAsync(id, cancellationToken);
        }

        private string GetUserId() => User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
    }
}
