using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using YouTubeV2.Api.Attributes;
using YouTubeV2.Application.DTO.PlaylistDTOS;
using YouTubeV2.Application.DTO.UserDTOS;
using YouTubeV2.Application.Migrations;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Services;

namespace YouTubeV2.Api.Controllers
{
    [Roles(Role.Simple, Role.Creator, Role.Administrator)]
    [ApiController]
    [Route("playlist")]
    public class PlaylistsController : Controller
    {
        private readonly IPlaylistService _playlistsService;

        public PlaylistsController(IPlaylistService playlistsService)
        {
            _playlistsService = playlistsService;
        }
        [HttpPost("details")]
        public async Task<ActionResult<CreatePlaylistResponseDto>> CreatePlaylist(CreatePlaylistRequestDto request, CancellationToken cancellationToken)
        {
            return Ok(await _playlistsService.CreatePlaylist(new Guid(GetUserId()), request, cancellationToken));
        }

        [HttpPut("details")]
        public async Task<ActionResult<UserDto>> UpdatePlaylistDetails([FromQuery][Required] Guid id, PlaylistEditDto request, CancellationToken cancellationToken)
        {
            return Ok(await _playlistsService.UpdatePlaylistDetails(new Guid(GetUserId()), id, request, cancellationToken));
        }

        [HttpDelete("details")]
        public async Task<IActionResult> DeletePlaylist([FromQuery][Required] Guid playlistId, CancellationToken cancellationToken)
        {
            await _playlistsService.DeletePlaylist(new Guid(GetUserId()), playlistId, cancellationToken);

            return Ok();
        }

        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<PlaylistBaseDto>>> GetUserPlaylists([FromQuery] Guid? id, CancellationToken cancellationToken)
        {
            if (id is null || id == Guid.Empty)
            {
                id = new Guid(GetUserId());
            }

            return Ok(await _playlistsService.GetUserPlaylists(new Guid(GetUserId()), id.Value, cancellationToken));
        }

        [HttpGet("video")]
        public async Task<ActionResult<PlaylistDto>> GetPlaylistVideos([FromQuery][Required] Guid id, CancellationToken cancellationToken)
        {
            return Ok(await _playlistsService.GetPlaylistVideos(new Guid(GetUserId()), id, cancellationToken));
        }

        [HttpPost("{id}/{videoId}")]
        public async Task<IActionResult> PlaylistPostVideo(Guid id, Guid videoId, CancellationToken cancellationToken)
        {
            await _playlistsService.PlaylistPostVideo(new Guid(GetUserId()), id, videoId, cancellationToken);

            return Ok();
        }

        [HttpDelete("{id}/{videoId}")]
        public async Task<IActionResult> PlaylistDeleteVideo(Guid id, Guid videoId, CancellationToken cancellationToken)
        {
            await _playlistsService.PlaylistDeleteVideo(new Guid(GetUserId()), id, videoId, cancellationToken);

            return Ok();
        }

        [HttpGet("recommended")]
        public async Task<ActionResult<PlaylistDto>> GetRecommendedPlaylist(CancellationToken cancellationToken)
        {
            return Ok(await _playlistsService.GetRecommendedPlaylist(new Guid(GetUserId()), cancellationToken));
        }
        private string GetUserId() => User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
    }
}
