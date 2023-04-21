using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using YouTubeV2.Api.Enums;
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

        public async Task<CreatePlaylistResponseDto> CreatePlaylist(Guid requesterUserGuid, CreatePlaylistRequestDto request, CancellationToken cancellationToken)
        {
            var creator = await _userManager
                .FindByIdAsync(requesterUserGuid.ToString())
                ?? throw new BadRequestException();

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

        public async Task DeletePlaylist(Guid requesterUserGuid, Guid playlistId, CancellationToken cancellationToken)
        {
            var playlist = await _context.Playlists
               .Include(p => p.Creator)
               .Include(p => p.Videos)
               .SingleOrDefaultAsync(p => p.Id == playlistId, cancellationToken)
               ?? throw new BadRequestException();

            if(playlist.Creator.Id.ToUpper() != requesterUserGuid.ToString().ToUpper())
            {
                throw new ForbiddenException();
            }

            playlist.Videos.Clear();
            await _context.SaveChangesAsync(cancellationToken);
            _context.Playlists.Remove(playlist);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<PlaylistDto> GetPlaylistVideos(string requesterUserId, Guid playlistId, CancellationToken cancellationToken)
        {
            var playlist = await _context.Playlists
                .Include(p => p.Creator)
                .Include(p => p.Videos)
                .SingleOrDefaultAsync(p => p.Id == playlistId, cancellationToken)
                ?? throw new BadRequestException();

            if (playlist.Visibility == Visibility.Private && 
                playlist.Creator.Id.ToUpper() != requesterUserId.ToUpper())
            {
                throw new ForbiddenException();
            }

            return new PlaylistDto(
                playlist.Name,
                playlist.Visibility,
                playlist.Videos.Select(
                    v => new VideoBaseDto(
                        v.Id.ToString(),
                        v.Title,
                        v.Duration,
                        _blobImageService.GetVideoThumbnail(v.Id.ToString()).ToString(),
                        v.Description,
                        v.UploadDate.ToString(),
                        v.ViewCount)
                    )
                );
        }

        public async Task<PlaylistDto> GetRecommendedPlaylist(Guid userGuid, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userGuid.ToString());

            var videos = _context.Videos
                .Where(v => v.Visibility == Visibility.Public)
                .OrderBy(v => Guid.NewGuid())
                .Take(8)
                .ToList();

            var result = new PlaylistDto(
                user.UserName + "'s Playlist",
                Visibility.Private,
                videos.Select(
                    v => new VideoBaseDto(
                        v.Id.ToString(),
                        v.Title,
                        v.Duration,
                        _blobImageService.GetVideoThumbnail(v.Id.ToString()).ToString(),
                        v.Description,
                        v.UploadDate.ToString(),
                        v.ViewCount)
                    )
                );
            return result;
        }

        public async Task<IEnumerable<PlaylistBaseDto>> GetUserPlaylists(string requesterUserId, string userId, CancellationToken cancellationToken)
        {
            var userWithPlaylists = await _context.Users
                .Include(p => p.Playlists)
                .ThenInclude(p => p.Videos)
                .SingleOrDefaultAsync(p => p.Id == userId, cancellationToken)
                ?? throw new BadRequestException();

            if(requesterUserId == userId)
            {
                return userWithPlaylists.Playlists
                    .Select(p => new PlaylistBaseDto(p.Name, p.Videos.Count, p.Id.ToString())).ToList();
            }
            else
            {
                return userWithPlaylists.Playlists
                    .Where(p => p.Visibility == Visibility.Public)
                    .Select(p => new PlaylistBaseDto(p.Name, p.Videos.Count, p.Id.ToString())).ToList();
            }
        }

        public async Task PlaylistDeleteVideo(Guid requesterUserGuid, Guid playlistId, Guid videoId, CancellationToken cancellationToken)
        {
            var playlist = await _context.Playlists
                .Include(p => p.Creator)
                .Include(p => p.Videos)
                .SingleOrDefaultAsync(p => p.Id == playlistId, cancellationToken)
                ?? throw new BadRequestException();

            var video = await _context.Videos
                .SingleOrDefaultAsync(v => v.Id == videoId, cancellationToken)
                ?? throw new BadRequestException();

            if (playlist.Creator.Id.ToUpper() != requesterUserGuid.ToString().ToUpper())
            {
                throw new ForbiddenException();
            }

            if (!playlist.Videos.Contains(video))
            {
                throw new BadRequestException();
            }

            playlist.Videos.Remove(video);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task PlaylistPostVideo(Guid requesterUserGuid, Guid playlistId, Guid videoId, CancellationToken cancellationToken)
        {
            var playlist = await _context.Playlists
                .Include(p => p.Creator)
                .Include(p => p.Videos)
                .SingleOrDefaultAsync(p => p.Id == playlistId, cancellationToken)
                ?? throw new BadRequestException();

            var video = await _context.Videos
                .Include(p => p.User).
                SingleOrDefaultAsync(v => v.Id == videoId, cancellationToken)
                ?? throw new BadRequestException();

            if (playlist.Creator.Id.ToUpper() != requesterUserGuid.ToString().ToUpper())
            {
                throw new ForbiddenException();
            }

            if(video.Visibility == Visibility.Private &&
                video.User.Id.ToUpper() != requesterUserGuid.ToString().ToUpper())
            {
                throw new ForbiddenException();
            }

            if (playlist.Videos.Contains(video))
            {
                throw new BadRequestException();
            }

            playlist.Videos.Add(video);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<UserDto> UpdatePlaylistDetails(string requesterUserId, Guid playlistId, PlaylistEditDto request, CancellationToken cancellationToken)
        {
            var playlist = await _context.Playlists
                .Include(p => p.Creator)
                .ThenInclude(p => p.Subscriptions)
                .SingleAsync(p => p.Id == playlistId, cancellationToken)
                ?? throw new BadRequestException();

            if (playlist.Creator.Id.ToUpper() != requesterUserId.ToUpper())
            {
                throw new ForbiddenException();
            }

            playlist.Name = request.name;
            playlist.Visibility = request.visibility;
            await _context.SaveChangesAsync(cancellationToken);

            var creator = await _userManager.FindByIdAsync(playlist.Creator.Id);
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
