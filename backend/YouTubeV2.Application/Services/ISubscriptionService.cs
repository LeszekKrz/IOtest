using YouTubeV2.Application.DTO.SubscribtionDTOS;

namespace YouTubeV2.Application.Services
{
    public interface ISubscriptionService
    {
        Task<UserSubscribtionListDto> GetSubscriptionsAsync(Guid Id, CancellationToken cancellationToken);
        Task<int> GetSubscriptionCount(Guid id, CancellationToken cancellationToken);
        Task PostSubscriptionsAsync(Guid subscribeeGuid, Guid subscriberGuid, CancellationToken cancellationToken);
        Task DeleteSubscriptionsAsync(Guid subscribeeGuid, Guid subscriberGuid, CancellationToken cancellationToken);
    }
}
