using Microsoft.AspNetCore.Mvc;
using YouTubeV2.Application.DTO;
using YouTubeV2.Application.Services;

namespace YouTubeV2.Api.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ISubscriptionService _subscriptionsService;

        public UserController(IUserService userService, ISubscriptionService subscriptionsService)
        {
            _userService = userService;
            _subscriptionsService = subscriptionsService;
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
            int subs = await _subscriptionsService.GetSubscriptionCount(new Guid(id), cancellationToken);

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
        public async Task<VideoListDto> GetVideosAsync([FromQuery] string id, CancellationToken cancellationToken)
        {
            // This endpoint exists only to check if frontend works. It will need to be raplaced

            List<string> tags = new List<string>() { "tag1", "tag2", "tag3" };
            List<VideoMetadataDto> videos = new List<VideoMetadataDto>()
            {
                new VideoMetadataDto("k4l2h342kjh", "Title", "desc", "https://filesdevelop.blob.core.windows.net/videos/pobrane.jpg",
                "h43il5u435", "Mr. Beast", 439870324, tags, "public", "queued", DateTime.Now, DateTime.Now, "43:21"),
                new VideoMetadataDto("k4l2h342kjh", "Title2", "desc2", "https://filesdevelop.blob.core.windows.net/videos/Rank1Bridge.png",
                "h43il5u435", "Pewdiepie", 439870324, tags, "public", "queued", DateTime.Now, DateTime.Now, "43:21"),
                new VideoMetadataDto("k4l2h342kjh", "Title3", "desc3", "https://filesdevelop.blob.core.windows.net/videos/pobrane.jpg",
                "h43il5u435", "Idk", 439870324, tags, "public", "queued", DateTime.Now, DateTime.Now, "43:21"),
                new VideoMetadataDto("k4l2h342kjh", "Title", "desc", "https://filesdevelop.blob.core.windows.net/videos/Rank1Bridge.png",
                "h43il5u435", "Mr. Beast", 439870324, tags, "public", "queued", DateTime.Now, DateTime.Now, "43:21"),
                new VideoMetadataDto("k4l2h342kjh", "Title2", "desc2", "https://filesdevelop.blob.core.windows.net/videos/pobrane.jpg",
                "h43il5u435", "Pewdiepie", 439870324, tags, "public", "queued", DateTime.Now, DateTime.Now, "43:21"),
                new VideoMetadataDto("k4l2h342kjh", "Title3", "desc3", "https://filesdevelop.blob.core.windows.net/videos/Rank1Bridge.png",
                "h43il5u435", "Idk", 439870324, tags, "public", "queued", DateTime.Now, DateTime.Now, "43:21"),
                new VideoMetadataDto("k4l2h342kjh", "Title", "desc", "https://filesdevelop.blob.core.windows.net/videos/pobrane.jpg",
                "h43il5u435", "Mr. Beast", 439870324, tags, "public", "queued", DateTime.Now, DateTime.Now, "43:21"),
                new VideoMetadataDto("k4l2h342kjh", "Title2", "desc2", "https://filesdevelop.blob.core.windows.net/videos/Rank1Bridge.png",
                "h43il5u435", "Pewdiepie", 439870324, tags, "public", "queued", DateTime.Now, DateTime.Now, "43:21"),
                new VideoMetadataDto("k4l2h342kjh", "Title3", "desc3", "https://filesdevelop.blob.core.windows.net/videos/pobrane.jpg",
                "h43il5u435", "Idk", 439870324, tags, "public", "queued", DateTime.Now, DateTime.Now, "43:21"),
                new VideoMetadataDto("k4l2h342kjh", "Title", "desc", "https://filesdevelop.blob.core.windows.net/videos/Rank1Bridge.png",
                "h43il5u435", "Mr. Beast", 439870324, tags, "public", "queued", DateTime.Now, DateTime.Now, "43:21"),
                new VideoMetadataDto("k4l2h342kjh", "Title2", "desc2", "https://filesdevelop.blob.core.windows.net/videos/pobrane.jpg",
                "h43il5u435", "Pewdiepie", 439870324, tags, "public", "queued", DateTime.Now, DateTime.Now, "43:21"),
                new VideoMetadataDto("k4l2h342kjh", "Title3", "desc3", "https://filesdevelop.blob.core.windows.net/videos/Rank1Bridge.png",
                "h43il5u435", "Idk", 439870324, tags, "public", "queued", DateTime.Now, DateTime.Now, "43:21"),
            };

            return new VideoListDto(videos);
        }
    }
}
