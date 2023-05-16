using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using YouTubeV2.Application.Services.BlobServices;

namespace YouTubeV2.Api.Controllers
{
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IBlobImageService _blobImageService;

        public ImageController(IBlobImageService blobImageService)
        {
            _blobImageService = blobImageService;
        }

        [HttpGet("api/images/{blobContainerName}/{fileName}")]
        public async Task<ActionResult> GetImageAsync(string blobContainerName, string fileName, CancellationToken cancellationToken)
        {
            if (blobContainerName.IsNullOrEmpty() || fileName.IsNullOrEmpty()) return BadRequest();

            BlobFile blobFile = await _blobImageService.GetImageAsync(blobContainerName, fileName, cancellationToken);

            return File(blobFile.Content, blobFile.ContentType, false);
        }
    }
}
