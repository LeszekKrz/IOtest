using YouTubeV2.Application.DTO.PlaylistDTOS;
using YouTubeV2.Application.DTO.UserDTOS;

namespace YouTubeV2.Application.Services
{
    public interface IPlaylistService
    {
        Task<CreatePlaylistResponseDto> CreatePlaylist(Guid requesterUserGuid, CreatePlaylistRequestDto request, CancellationToken cancellationToken);
        Task<UserDto> UpdatePlaylistDetails(Guid requesterUserGuid, Guid playlistId, PlaylistEditDto request, CancellationToken cancellationToken);
        Task DeletePlaylist(Guid requesterUserGuid, Guid playlistId, CancellationToken cancellationToken);
        Task<IEnumerable<PlaylistBaseDto>> GetUserPlaylists(Guid requesterUserGuid, Guid userGuid, CancellationToken cancellationToken);
        Task PlaylistPostVideo(Guid requesterUserGuid, Guid playlistId, Guid videoId, CancellationToken cancellationToken);
        Task PlaylistDeleteVideo(Guid requesterUserGuid, Guid playlistId, Guid videoId, CancellationToken cancellationToken);
        Task<PlaylistDto> GetPlaylistVideos(Guid requesterUserGuid, Guid playlistId, CancellationToken cancellationToken);
        Task<PlaylistDto> GetRecommendedPlaylist(Guid requesterUserGuid, CancellationToken cancellationToken);
    }
}
