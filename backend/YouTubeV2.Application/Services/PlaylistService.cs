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
    public class PlaylistService : IPlaylistService
    {
        public async Task<ActionResult<CreatePlaylistResponseDto>> CreatePlaylist(CreatePlaylistRequestDto request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task DeletePlaylist([FromQuery, Required] Guid id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<PlaylistDto> GetPlaylistVideos(Guid playlistId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<PlaylistDto> GetRecommendedPlaylist(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<ActionResult<IEnumerable<PlaylistBaseDto>>> GetUserPlaylists([FromQuery, Required] Guid userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task PlaylistDeleteVideo([FromQuery, Required] Guid playlistId, [FromQuery, Required] Guid videoId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task PlaylistPostVideo([FromQuery, Required] Guid playlistId, [FromQuery, Required] Guid videoId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<ActionResult<UserDto>> UpdatePlaylistDetails([FromQuery, Required] Guid id, PlaylistEditDto request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
