using Microsoft.AspNetCore.Mvc;
using YouTubeV2.Application.DTO.UserDTOS;
using YouTubeV2.Application.DTO.VideoDTOS;
using YouTubeV2.Application.Services;
using YouTubeV2.Application.Services.VideoServices;

namespace YouTubeV2.Api.Controllers
{
    [ApiController]
    public class UserController : IdentityControllerBase
    {
        private readonly IUserService _userService;
        private readonly ISubscriptionService _subscriptionsService;
        private readonly IVideoService _videoService;

        public UserController(IUserService userService, ISubscriptionService subscriptionsService, IVideoService videoService)
        {
            _userService = userService;
            _subscriptionsService = subscriptionsService;
            _videoService = videoService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto registerDto, CancellationToken cancellationToken)
        {
            await _userService.RegisterAsync(registerDto, cancellationToken);

            return Ok();
        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken)
        {
            LoginResponseDto loginResponseDto = await _userService.LoginAsync(loginDto, cancellationToken);

            return Ok(loginResponseDto);
        }

        [HttpGet("user")]
        public async Task<UserDto> GetUserAsync([FromQuery] string id, CancellationToken cancellationToken)
        {
            // This endpoint exists only to check if frontend works. It will need to be raplaced
            int subs = await _subscriptionsService.GetSubscriptionCount(id, cancellationToken);

            return new UserDto(id,
              "john.doe@mail.com",
              "johnny123",
              "John",
              "Doe",
              10,
              "Simple",
              "https://filesdevelop.blob.core.windows.net/useravatars/53_square.jpg",
              subs);
        }

        [HttpGet("user/videos")]
        public async Task<ActionResult<VideoListDto>> GetUserVideosAsync([FromQuery] string? id, CancellationToken cancellationToken)
        {
            string? userId = GetUserId();
            if (userId is null) return Forbid();

            return userId == id || id is null
                ? await _videoService.GetAllUserVideos(userId, cancellationToken)
                : await _videoService.GetAllAvailableUserVideos(id, cancellationToken);
        }
    }
}
