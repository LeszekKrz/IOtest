using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using YouTubeV2.Application.DTO.PlaylistDTOS;
using YouTubeV2.Application.DTO.UserDTOS;
using YouTubeV2.Application.DTO.VideoDTOS;
using YouTubeV2.Application.Exceptions;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Services.BlobServices;

namespace YouTubeV2.Application.Services
{
    public class PlaylistService : IPlaylistService
    {
        private readonly YTContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IBlobImageService _blobImageService;
        public PlaylistService(YTContext context, UserManager<User> userManager, IBlobImageService blobImageService)
        {
            _context = context;
            _userManager = userManager;
            _blobImageService = blobImageService;
        }

        public async Task<CreatePlaylistResponseDto> CreatePlaylist(Guid userGuid, CreatePlaylistRequestDto request, CancellationToken cancellationToken)
        {
            var creator = await _userManager.FindByIdAsync(userGuid.ToString()) ?? throw new BadRequestException();

            var playlist = new Playlist
            {
                Visibility = request.visibility,
                Name = request.name,
                Creator = creator
            };
            var entity = await _context.Playlists.AddAsync(playlist, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new CreatePlaylistResponseDto(entity.Entity.Id.ToString());
        }

        public async Task DeletePlaylist(Guid playlistId, CancellationToken cancellationToken)
        {
            var playlist = await _context.Playlists.Include(p => p.Creator)
               .Include(p => p.Videos).SingleOrDefaultAsync(p => p.Id == playlistId, cancellationToken)
               ?? throw new BadRequestException();

            playlist.Videos.Clear();
            await _context.SaveChangesAsync(cancellationToken);
            _context.Playlists.Remove(playlist);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<PlaylistDto> GetPlaylistVideos(Guid playlistId, CancellationToken cancellationToken)
        {
            var playlist = await _context.Playlists.Include(p => p.Creator)
                .Include(p => p.Videos).SingleOrDefaultAsync(p => p.Id == playlistId, cancellationToken)
                ?? throw new BadRequestException();

            var videoDtoList = new List<VideoBaseDto>();
            foreach (var video in playlist.Videos)
            {
                videoDtoList.Add(
                    new VideoBaseDto(
                        video.Id.ToString(),
                        video.Title,
                        video.Duration,
                        _blobImageService.GetVideoThumbnail(video.Id.ToString()).ToString(),
                        video.Description,
                        video.UploadDate.ToString(),
                        video.ViewCount)
                    );
            }
            PlaylistDto result = new(
                playlist.Name,
                playlist.Visibility,
                videoDtoList);

            return result;
        }

        public async Task<PlaylistDto> GetRecommendedPlaylist(CancellationToken cancellationToken)
        {
            //future work
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<PlaylistBaseDto>> GetUserPlaylists(Guid userId, CancellationToken cancellationToken)
        {
            var userWithPlaylists = await _context.Users.Include(p => p.Playlists)
                .ThenInclude(p => p.Videos)
                .SingleOrDefaultAsync(p => p.Id == userId.ToString(), cancellationToken)
                ?? throw new BadRequestException();

            var result = new List<PlaylistBaseDto>();
            foreach (var playlist in userWithPlaylists.Playlists)
            {
                result.Add(new PlaylistBaseDto(playlist.Name, playlist.Videos.Count, playlist.Id.ToString()));
            }
            return result;
        }

        public async Task PlaylistDeleteVideo(Guid playlistId, Guid videoId, CancellationToken cancellationToken)
        {
            var playlist = await _context.Playlists.Include(p => p.Creator)
                .Include(p => p.Videos).SingleOrDefaultAsync(p => p.Id == playlistId, cancellationToken)
                ?? throw new BadRequestException();
            var video = await _context.Videos.SingleOrDefaultAsync(v => v.Id == videoId, cancellationToken)
                ?? throw new BadRequestException();

            if (playlist == null || video == null)
            {
                throw new BadRequestException();
            }

            if (!playlist.Videos.Contains(video))
            {
                throw new BadRequestException();
            }

            playlist.Videos.Remove(video);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task PlaylistPostVideo(Guid playlistId, Guid videoId, CancellationToken cancellationToken)
        {
            var playlist = await _context.Playlists.Include(p => p.Creator)
                .Include(p => p.Videos).SingleOrDefaultAsync(p => p.Id == playlistId, cancellationToken)
                ?? throw new BadRequestException();

            var video = await _context.Videos.SingleOrDefaultAsync(v => v.Id == videoId, cancellationToken)
                ?? throw new BadRequestException();

            if (playlist.Videos.Contains(video))
            {
                throw new BadRequestException();
            }

            playlist.Videos.Add(video);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<UserDto> UpdatePlaylistDetails([FromQuery, Required] Guid playlistId, PlaylistEditDto request, CancellationToken cancellationToken)
        {
            var playlist = await _context.Playlists.Include(p => p.Creator).ThenInclude(p => p.Subscriptions)
                .SingleAsync(p => p.Id == playlistId, cancellationToken);

            if(playlist == null)
            {
                throw new BadRequestException();
            }
            playlist.Name = request.name;
            playlist.Visibility = request.visibility;
            await _context.SaveChangesAsync(cancellationToken);

            var creator = await _userManager.FindByIdAsync(playlist.Creator.Id) ?? throw new Exception();
            var roles = await _userManager.GetRolesAsync(creator);

            var result = new UserDto(
                playlist.Creator.Id,
                playlist.Creator.Email,
                playlist.Creator.UserName,
                playlist.Creator.Name,
                playlist.Creator.Surname,
                0.0,
                roles[0],
                _blobImageService.GetProfilePicture(playlist.Creator.Id).ToString(),
                playlist.Creator.Subscriptions.Count);
            return result;
        }
    }
}
