using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouTubeV2.Application.DTO.PlaylistDTOS;
using YouTubeV2.Application.DTO.UserDTOS;

namespace YouTubeV2.Application.Services
{
    public interface IPlaylistService
    {
        Task<ActionResult<CreatePlaylistResponseDto>> CreatePlaylist(CreatePlaylistRequestDto request, CancellationToken cancellationToken);
        Task<ActionResult<UserDto>> UpdatePlaylistDetails([FromQuery][Required] Guid id, PlaylistEditDto request, CancellationToken cancellationToken);
        Task DeletePlaylist([FromQuery][Required] Guid id, CancellationToken cancellationToken);
        Task<ActionResult<IEnumerable<PlaylistBaseDto>>> GetUserPlaylists([FromQuery][Required] Guid userId, CancellationToken cancellationToken);
        Task PlaylistPostVideo([FromQuery][Required] Guid playlistId, [FromQuery][Required] Guid videoId, CancellationToken cancellationToken);
        Task PlaylistDeleteVideo([FromQuery][Required] Guid playlistId, [FromQuery][Required] Guid videoId, CancellationToken cancellationToken);
        Task<PlaylistDto> GetPlaylistVideos(Guid playlistId, CancellationToken cancellationToken);
        Task<PlaylistDto> GetRecommendedPlaylist(CancellationToken cancellationToken);
    }
}
