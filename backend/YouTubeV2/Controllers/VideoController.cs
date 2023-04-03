using Microsoft.AspNetCore.Mvc;
using YouTubeV2.Api.Attributes;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Services.AzureServices.BlobServices;

namespace YouTubeV2.Api.Controllers
{
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly IBlobVideoService _blobVideoService;

        public VideoController(IBlobVideoService blobVideoService)
        {
            _blobVideoService = blobVideoService;
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
    }
}
