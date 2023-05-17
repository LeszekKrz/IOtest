using YouTubeV2.Application.DTO.PlaylistDTOS;
using YouTubeV2.Application.DTO.UserDTOS;

namespace YouTubeV2.Application.Services
{
    public interface IPlaylistService
    {
        Task<CreatePlaylistResponseDto> CreatePlaylist(string requesterUserId, CreatePlaylistRequestDto request, CancellationToken cancellationToken);
        Task<UserDto> UpdatePlaylistDetails(string requesterUserId, Guid playlistId, PlaylistEditDto request, CancellationToken cancellationToken);
        Task DeletePlaylist(string requesterUserId, Guid playlistId, CancellationToken cancellationToken);
        Task<IEnumerable<PlaylistBaseDto>> GetUserPlaylists(string requesterUserId, string userId, CancellationToken cancellationToken);
        Task PlaylistPostVideo(string requesterUserId, Guid playlistId, Guid videoId, CancellationToken cancellationToken);
        Task PlaylistDeleteVideo(string requesterUserId, Guid playlistId, Guid videoId, CancellationToken cancellationToken);
        Task<PlaylistDto> GetPlaylistVideos(string requesterUserId, Guid playlistId, CancellationToken cancellationToken);
        Task<PlaylistDto> GetRecommendedPlaylist(string requesterUserId, CancellationToken cancellationToken);
    }
}
