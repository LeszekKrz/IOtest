﻿using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;
using YouTubeV2.Application.DTO.VideoMetadataDTOS;
using YouTubeV2.Application.Enums;
using YouTubeV2.Application.Model;

namespace YouTubeV2.Application.Services.VideoServices
{
    public interface IVideoService
    {
        Task<Guid> AddVideoMetadataAsync(VideoMetadataPostDto videoMetadata, User user, CancellationToken cancellationToken = default);

        Task<Video?> GetVideoByIdAsync(Guid id, CancellationToken cancellationToken = default, params Expression<Func<Video, object>>[] includes);

        Task SetVideoProcessingProgressAsync(Video video, ProcessingProgress processingProgress, CancellationToken cancellationToken = default);

        Task<VideoMetadataDto> GetVideoMetadataAsync(Guid id, CancellationToken cancellationToken = default);

        Task AuthorizeVideoAccessAsync(Guid videoId, string userId, CancellationToken cancellationToken = default);

        Task SetVideoLengthAsync(Video video, double length, CancellationToken cancellationToken = default);
    }
}
