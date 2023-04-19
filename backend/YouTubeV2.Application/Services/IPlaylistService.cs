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
        Task<CreatePlaylistResponseDto> CreatePlaylist(Guid userGuid, CreatePlaylistRequestDto request, CancellationToken cancellationToken);
        Task<UserDto> UpdatePlaylistDetails(Guid id, PlaylistEditDto request, CancellationToken cancellationToken);
        Task DeletePlaylist(Guid playlistId, CancellationToken cancellationToken);
        Task<IEnumerable<PlaylistBaseDto>> GetUserPlaylists(Guid userId, CancellationToken cancellationToken);
        Task PlaylistPostVideo(Guid playlistId, Guid videoId, CancellationToken cancellationToken);
        Task PlaylistDeleteVideo(Guid playlistId, Guid videoId, CancellationToken cancellationToken);
        Task<PlaylistDto> GetPlaylistVideos(Guid playlistId, CancellationToken cancellationToken);
        Task<PlaylistDto> GetRecommendedPlaylist(CancellationToken cancellationToken);
    }
}
