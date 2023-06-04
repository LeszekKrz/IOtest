using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using YouTubeV2.Api.Attributes;
using YouTubeV2.Application.DTO.PlaylistDTOS;
using YouTubeV2.Application.DTO.UserDTOS;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Services;

namespace YouTubeV2.Api.Controllers
{
    [Roles(Role.Simple, Role.Creator, Role.Administrator)]
    [ApiController]
    [Route("api/playlist")]
    public class PlaylistsController : IdentityControllerBase
    {
        private readonly IPlaylistService _playlistsService;

        public PlaylistsController(IPlaylistService playlistsService)
        {
            _playlistsService = playlistsService;
        }

        [HttpPost("details")]
        public async Task<ActionResult<CreatePlaylistResponseDto>> CreatePlaylist(CreatePlaylistRequestDto request, CancellationToken cancellationToken)
        {
            string? userId = GetUserId();
            if (userId is null) return Forbid();

            return Ok(await _playlistsService.CreatePlaylist(userId, request, cancellationToken));
        }

        [HttpPut("details")]
        public async Task<ActionResult<UserDto>> UpdatePlaylistDetails([FromQuery][Required] Guid id, PlaylistEditDto request, CancellationToken cancellationToken)
        {
            string? userId = GetUserId();
            if (userId is null) return Forbid();

            return Ok(await _playlistsService.UpdatePlaylistDetails(userId, id, request, cancellationToken));
        }

        [HttpDelete("details")]
        public async Task<IActionResult> DeletePlaylist([FromQuery][Required] Guid id, CancellationToken cancellationToken)
        {
            string? userId = GetUserId();
            if (userId is null) return Forbid();

            await _playlistsService.DeletePlaylist(userId, id, cancellationToken);

            return Ok();
        }

        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<PlaylistBaseDto>>> GetUserPlaylists([FromQuery] Guid? id, CancellationToken cancellationToken)
        {
            string? userId = GetUserId();
            if (userId is null) return Forbid();

            if (id is null || id == Guid.Empty)
            {
                id = new Guid(userId);
            }

            return Ok(await _playlistsService.GetUserPlaylists(userId, id.Value.ToString(), cancellationToken));
        }

        [HttpGet("video")]
        public async Task<ActionResult<PlaylistDto>> GetPlaylistVideos([FromQuery][Required] Guid id, CancellationToken cancellationToken)
        {
            string? userId = GetUserId();
            if (userId is null) return Forbid();

            return Ok(await _playlistsService.GetPlaylistVideos(userId, id, cancellationToken));
        }

        [HttpPost("{id}/{videoId}")]
        public async Task<IActionResult> PlaylistPostVideo(Guid id, Guid videoId, CancellationToken cancellationToken)
        {
            string? userId = GetUserId();
            if (userId is null) return Forbid();

            await _playlistsService.PlaylistPostVideo(userId, id, videoId, cancellationToken);

            return Ok();
        }

        [HttpDelete("{id}/{videoId}")]
        public async Task<IActionResult> PlaylistDeleteVideo(Guid id, Guid videoId, CancellationToken cancellationToken)
        {
            string? userId = GetUserId();
            if (userId is null) return Forbid();

            await _playlistsService.PlaylistDeleteVideo(userId, id, videoId, cancellationToken);

            return Ok();
        }

        [HttpGet("recommended")]
        public async Task<ActionResult<PlaylistDto>> GetRecommendedPlaylist(CancellationToken cancellationToken)
        {
            string? userId = GetUserId();
            if (userId is null) return Forbid();

            return Ok(await _playlistsService.GetRecommendedPlaylist(userId, cancellationToken));
        }
    }
}
