using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using YouTubeV2.Application.DTO.PlaylistDTOS;
using YouTubeV2.Application.DTO.UserDTOS;
using YouTubeV2.Application.DTO.VideoDTOS;
using YouTubeV2.Application.Exceptions;
using YouTubeV2.Application.Model;

namespace YouTubeV2.Application.Services
{
    public class PlaylistService : IPlaylistService
    {
        private readonly YTContext _context;
        private readonly UserManager<User> _userManager;

        public PlaylistService(YTContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<CreatePlaylistResponseDto> CreatePlaylist(Guid userGuid, CreatePlaylistRequestDto request, CancellationToken cancellationToken)
        {
            var creator = await _userManager.FindByIdAsync(userGuid.ToString());
            if(creator == null)
            {
                throw new BadRequestException();
            }
            var playlist = new Playlist
            {
                Visibility = request.visibility,
                Name = request.name,
                Creator = creator
            };
            var entity = await _context.Playlists.AddAsync(playlist, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            //check entity somehow???
            return new CreatePlaylistResponseDto(entity.Entity.Id.ToString());
        }

        public async Task DeletePlaylist(Guid playlistId, CancellationToken cancellationToken)
        {
            var playlist = await _context.Playlists.Include(p => p.Creator)
               .Include(p => p.Videos).SingleAsync(p => p.Id == playlistId, cancellationToken);

            if (playlist == null)
            {
                throw new BadRequestException();
            }
            //are 2 lines below necessary??
            playlist.Videos.Clear();
            await _context.SaveChangesAsync(cancellationToken);
            _context.Playlists.Remove(playlist);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<PlaylistDto> GetPlaylistVideos(Guid playlistId, CancellationToken cancellationToken)
        {
            var playlist = await _context.Playlists.Include(p => p.Creator)
                .Include(p => p.Videos).SingleAsync(p => p.Id == playlistId, cancellationToken);

            if (playlist == null)
            {
                throw new BadRequestException();
            }

            //i should probably use some constructor or playlist -> playlistDto transforer
            var videoDtoList = new List<VideoBaseDto>();
            foreach (var video in playlist.Videos)
            {
                VideoBaseDto videoDto = new(
                    video.Id.ToString(),
                    video.Title,
                    video.Duration,
                    //how to access thumbnails for each video in playlist.Videos???
                    "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVQYV2NgYAAAAAMAAWgmWQ0AAAAASUVORK5CYII=",
                    video.Description,
                    video.UploadDate.ToString(),
                    video.ViewCount);
                videoDtoList.Add(videoDto);
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
                .SingleAsync(p => p.Id == userId.ToString(), cancellationToken);

            if(userWithPlaylists == null)
            {
                throw new BadRequestException();
            }

            var result = new List<PlaylistBaseDto>();
            foreach(var playlist in userWithPlaylists.Playlists)
            {
                PlaylistBaseDto playlistBaseDto = new(playlist.Name, playlist.Videos.Count, playlist.Id.ToString());
                result.Add(playlistBaseDto);
            }

            return result;
        }

        public async Task PlaylistDeleteVideo(Guid playlistId, Guid videoId, CancellationToken cancellationToken)
        {
            //500 on videoId is playlistId
            var playlist = await _context.Playlists.Include(p => p.Creator)
                .Include(p => p.Videos).SingleAsync(p => p.Id == playlistId, cancellationToken);
            var video = await _context.Videos.SingleAsync(v => v.Id == videoId, cancellationToken);

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
            //500 on videoId is playlistId
            var playlist = await _context.Playlists.Include(p => p.Creator)
                .Include(p => p.Videos).SingleAsync(p => p.Id == playlistId, cancellationToken);
            var video = await _context.Videos.SingleAsync(v => v.Id == videoId, cancellationToken);

            if (playlist == null || video == null)
            {
                throw new BadRequestException();
            }

            if (playlist.Videos.Contains(video))
            {
                throw new BadRequestException();
            }

            playlist.Videos.Add(video);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<UserDto> UpdatePlaylistDetails([FromQuery, Required] Guid playlistId, PlaylistEditDto request, CancellationToken cancellationToken)
        {
            var playlist = await _context.Playlists.Include(p => p.Creator)
                .SingleAsync(p => p.Id == playlistId, cancellationToken);

            if(playlist == null)
            {
                throw new BadRequestException();
            }
            playlist.Name = request.name;
            playlist.Visibility = request.visibility;
            await _context.SaveChangesAsync(cancellationToken);

            // also why return userDto???
            throw new NotImplementedException();
        }
    }
}
