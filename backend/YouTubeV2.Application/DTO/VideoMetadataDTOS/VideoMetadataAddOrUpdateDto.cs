﻿using YouTubeV2.Api.Enums;

namespace YouTubeV2.Application.DTO.VideoMetadataDTOS
{
    public record VideoMetadataAddOrUpdateDto(
        string title,
        string description,
        string thumbnail,
        IReadOnlyCollection<string> tags,
        Visibility visibility);
}
