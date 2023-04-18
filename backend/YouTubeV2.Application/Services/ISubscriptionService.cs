using YouTubeV2.Application.DTO.SubscriptionDTOS;

namespace YouTubeV2.Application.Services
{
    public interface ISubscriptionService
    {
        Task<UserSubscriptionListDTO> GetSubscriptionsAsync(string id, CancellationToken cancellationToken = default);
        Task<int> GetSubscriptionCount(string id, CancellationToken cancellationToken = default);
        Task AddSubscriptionAsync(string subscribeeId, string subscriberId, CancellationToken cancellationToken = default);
        Task DeleteSubscriptionAsync(string subscribeeId, string subscriberId, CancellationToken cancellationToken = default);
    }
}
