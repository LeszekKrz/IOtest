﻿using YouTubeV2.Application.DTO.ReactionDTOS;
using YouTubeV2.Application.Enums;
using YouTubeV2.Application.Model;

namespace YouTubeV2.Application.Services
{
    public interface IReactionService
    {
        Task AddOrUpdateReactionAsync(ReactionType reactionType, Video video, User user, CancellationToken cancellationToken = default);

        Task<ReactionsDto> GetReactionsAsync(Video video, User user, CancellationToken cancellationToken = default);
    }
}
