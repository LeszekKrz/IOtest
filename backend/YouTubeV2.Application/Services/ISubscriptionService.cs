using YouTubeV2.Application.DTO.SubscriptionDTOS;
using YouTubeV2.Application.Model;

namespace YouTubeV2.Application.Services
{
    public interface ISubscriptionService
    {
        Task<UserSubscriptionListDto> GetSubscriptionsAsync(string id, CancellationToken cancellationToken = default);

        Task<int> GetSubscriptionCountAsync(string id, CancellationToken cancellationToken = default);

        Task AddSubscriptionAsync(string subscribeeId, string subscriberId, CancellationToken cancellationToken = default);

        Task DeleteSubscriptionAsync(string subscribeeId, string subscriberId, CancellationToken cancellationToken = default);

        Task DeleteAllSubscriptionsRelatedToUserAsync(User user, CancellationToken cancellationToken = default);

        Task DeleteAllSubscriptionsWhereUserIsSubscribeeAsync(User user, CancellationToken cancellationToken = default);
    }
}
