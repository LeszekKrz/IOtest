using YouTubeV2.Application.DTO.SubscriptionDTOS;

namespace YouTubeV2.Application.Services
{
    public interface ISubscriptionService
    {
        Task<UserSubscriptionListDto> GetSubscriptionsAsync(Guid Id, CancellationToken cancellationToken);
        Task<int> GetSubscriptionCount(Guid id, CancellationToken cancellationToken);
        Task PostSubscriptionsAsync(Guid subscribeeGuid, Guid subscriberGuid, CancellationToken cancellationToken);
        Task DeleteSubscriptionsAsync(Guid subscribeeGuid, Guid subscriberGuid, CancellationToken cancellationToken);
    }
}
