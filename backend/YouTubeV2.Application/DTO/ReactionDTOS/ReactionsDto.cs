using YouTubeV2.Application.Enums;

namespace YouTubeV2.Application.DTO.ReactionDTOS
{
    public record class ReactionsDto(int positiveCount, int negativeCount, ReactionType currentUserReaction);
}
