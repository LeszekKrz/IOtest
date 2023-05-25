using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using YouTubeV2.Api.Attributes;
using YouTubeV2.Application.DTO.ReactionDTOS;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Services;
using YouTubeV2.Application.Services.VideoServices;

namespace YouTubeV2.Api.Controllers
{
    [ApiController]
    [Route("api/video-reaction")]
    [Roles(Role.Simple, Role.Creator, Role.Administrator)]
    public class ReactionController : IdentityControllerBase
    {
        private readonly IVideoService _videoService;
        private readonly IReactionService _reactionService;
        private readonly IUserService _userService;

        public ReactionController(IVideoService videoService, IReactionService reactionService, IUserService userService)
        {
            _videoService = videoService;
            _reactionService = reactionService;
            _userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult> AddOrUpdateReactionAsync([FromQuery][Required] Guid id, [FromBody] AddReactionDto addReactionDto, CancellationToken cancellationToken)
        {
            string? userId = GetUserId();
            if (userId is null) return Forbid();

            Video? video = await _videoService.GetVideoByIdAsync(id, cancellationToken);
            if (video == null) return NotFound($"Video with id {id} not found");

            User? user = await _userService.GetByIdAsync(userId);
            if (user == null) return NotFound($"User with id {id} not found");

            await _reactionService.AddOrUpdateReactionAsync(addReactionDto.value, video, user, cancellationToken);
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<ReactionsDto>> GetReactionsAsync([FromQuery][Required] Guid id, CancellationToken cancellationToken)
        {
            string? userId = GetUserId();
            if (userId is null) return Forbid();

            Video? video = await _videoService.GetVideoByIdAsync(id, cancellationToken);
            if (video == null) return NotFound($"Video with id {id} not found");

            User? user = await _userService.GetByIdAsync(userId);
            if (user == null) return NotFound($"User with id {id} not found");

            return await _reactionService.GetReactionsAsync(video, user, cancellationToken);
        }
    }
}
