using Microsoft.AspNetCore.Mvc;
using YouTubeV2.Application.DTO.UserDTOS;
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
    }
}
