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
    [Route("playlists")]
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
        public async Task<ActionResult<UserDto>> UpdatePlaylistDetails([FromQuery][Required] Guid playlistId, PlaylistEditDto request, CancellationToken cancellationToken)
        {
            return Ok(await _playlistsService.UpdatePlaylistDetails(playlistId, request, cancellationToken));
        }

        [HttpDelete("details")]
        public async Task<IActionResult> DeletePlaylist([FromQuery][Required] Guid playlistId, CancellationToken cancellationToken)
        {
            await _playlistsService.DeletePlaylist(playlistId, cancellationToken);

            return Ok();
        }

        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<PlaylistBaseDto>>> GetUserPlaylists([FromQuery][Required] Guid userId, CancellationToken cancellationToken)
        {
            return Ok(await _playlistsService.GetUserPlaylists(userId, cancellationToken));
        }

        [HttpGet("video")]
        public async Task<ActionResult<PlaylistDto>> GetPlaylistVideos([FromQuery][Required] Guid playlistId, CancellationToken cancellationToken)
        {
            return Ok(await _playlistsService.GetPlaylistVideos(playlistId, cancellationToken));
        }

        [HttpPost("{id}/{videoId}")]
        public async Task<IActionResult> PlaylistPostVideo(Guid playlistId, Guid videoId, CancellationToken cancellationToken)
        {
            await _playlistsService.PlaylistPostVideo(playlistId, videoId, cancellationToken);

            return Ok();
        }

        [HttpDelete("{id}/{videoId}")]
        public async Task<IActionResult> PlaylistDeleteVideo(Guid playlistId, Guid videoId, CancellationToken cancellationToken)
        {
            await _playlistsService.PlaylistDeleteVideo(playlistId, videoId, cancellationToken);

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
