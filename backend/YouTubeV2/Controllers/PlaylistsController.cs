using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using YouTubeV2.Application.DTO.PlaylistDTOS;
using YouTubeV2.Application.DTO.UserDTOS;
using YouTubeV2.Application.Services;
using YouTubeV2.Application.Services.JwtFeatures;

namespace YouTubeV2.Api.Controllers
{
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
            string jwtToken = HttpContext.Request.Headers["Authorization"].ToString();

            Guid userGuid = JwtHandler.ExtractUserGuidFromToken(jwtToken);

            return Ok(await _playlistsService.CreatePlaylist(userGuid, request, cancellationToken));
        }
        [HttpPut("details")]
        public async Task<ActionResult<UserDto>> UpdatePlaylistDetails([FromQuery][Required] Guid id, PlaylistEditDto request, CancellationToken cancellationToken)
        {
            return Ok(await _playlistsService.UpdatePlaylistDetails(id, request, cancellationToken));
        }
        [HttpDelete("details")]
        public async Task<IActionResult> DeletePlaylist([FromQuery][Required] Guid id, CancellationToken cancellationToken)
        {
            await _playlistsService.DeletePlaylist(id, cancellationToken);

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
        public async Task<IActionResult> PlaylistPostVideo(Guid id, Guid videoId, CancellationToken cancellationToken)
        {
            await _playlistsService.PlaylistPostVideo(id, videoId, cancellationToken);

            return Ok();
        }
        [HttpDelete("{id}/{videoId}")]
        public async Task<IActionResult> PlaylistDeleteVideo(Guid id, Guid videoId, CancellationToken cancellationToken)
        {
            await _playlistsService.PlaylistDeleteVideo(id, videoId, cancellationToken);

            return Ok();
        }
        [HttpGet("recommended")]
        public async Task<ActionResult<PlaylistDto>> GetRecommendedPlaylist(CancellationToken cancellationToken)
        {
            return Ok(await _playlistsService.GetRecommendedPlaylist(cancellationToken));
        }
    }
}
