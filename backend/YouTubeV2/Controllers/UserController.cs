using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using YouTubeV2.Api.Attributes;
using YouTubeV2.Application.DTO.UserDTOS;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Services;

namespace YouTubeV2.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class UserController : IdentityControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService, ISubscriptionService subscriptionsService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto registerDto, CancellationToken cancellationToken)
        {
            string createdID = await _userService.RegisterAsync(registerDto, cancellationToken);

            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken)
        {
            LoginResponseDto loginResponseDto = await _userService.LoginAsync(loginDto, cancellationToken);

            return Ok(loginResponseDto);
        }

        [HttpGet("user")]
        [Roles(Role.Simple, Role.Creator, Role.Administrator)]
        public async Task<ActionResult<UserDto>> GetAsync([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var callerID = GetUserId();
            if (callerID == null) return Forbid();

            string userID = id.ToString();
            if (id == Guid.Empty)
                userID = string.Empty;

            return Ok(await _userService.GetAsync(callerID, userID, cancellationToken));
        }

        [HttpPut("user")]
        [Roles(Role.Simple, Role.Creator, Role.Administrator)]
        public async Task<IActionResult> PutAsync([FromQuery] Guid id, [FromBody][Required] UpdateUserDto updateUserDTO, 
            CancellationToken cancellationToken)
        {
            var callerID = GetUserId();
            if (callerID == null) return Forbid();

            string userID = id.ToString();
            if (id == Guid.Empty)
                userID = string.Empty;

            await _userService.EditAsync(callerID, userID, updateUserDTO, cancellationToken);

            return Ok();
        }

        [HttpDelete("user")]
        [Roles(Role.Simple, Role.Creator, Role.Administrator)]
        public async Task<IActionResult> DeleteAsync([FromQuery][Required] Guid id, CancellationToken cancellationToken)
        {
            var callerID = GetUserId();
            if (callerID == null) return Forbid();

            string userID = id.ToString();
            if (id == Guid.Empty)
                userID = string.Empty;

            await _userService.DeleteAsync(callerID, userID, cancellationToken);

            return Ok();
        }
    }
}
