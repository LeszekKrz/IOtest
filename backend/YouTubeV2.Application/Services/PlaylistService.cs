using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouTubeV2.Api.Enums;
using YouTubeV2.Application.DTO.PlaylistDTOS;
using YouTubeV2.Application.DTO.UserDTOS;
using YouTubeV2.Application.DTO.VideoDTOS;
using YouTubeV2.Application.Exceptions;
using YouTubeV2.Application.Migrations;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Services.BlobServices;

namespace YouTubeV2.Application.Services
{
    public class PlaylistService : IPlaylistService
    {
        private readonly IBlobImageService _blobImageService;
        private readonly YTContext _context;
        private readonly UserManager<User> _userManager;

        public PlaylistService(IBlobImageService blobImageService, YTContext context, UserManager<User> userManager)
        {
            _blobImageService = blobImageService;
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
            var playlist = new Model.Playlist
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

        public async Task DeletePlaylist(Guid id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<PlaylistDto> GetPlaylistVideos(Guid playlistId, CancellationToken cancellationToken)
        {
            var playlist = await _context.Playlists.Include(p => p.Creator)
                .Include(p => p.Videos).SingleAsync(p => p.Id == playlistId, cancellationToken);
            if (playlist == null)
            {
                throw new BadRequestException();
            }
            //how to access thumbnails for each video in playlist.Videos???
            var videoDtoList = new List<VideoBaseDto>();
            foreach (var video in playlist.Videos)
            {
                VideoBaseDto videoDto = new(
                    video.Id.ToString(),
                    video.Title,
                    video.Duration,
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

        public async Task<UserDto> UpdatePlaylistDetails([FromQuery, Required] Guid id, PlaylistEditDto request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
