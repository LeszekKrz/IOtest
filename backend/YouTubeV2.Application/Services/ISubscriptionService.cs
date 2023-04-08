using YouTubeV2.Application.DTO;

namespace YouTubeV2.Application.Services
{
    public interface ISubscriptionService
    {
        public Task<UserSubscriptionListDTO> GetSubscriptionsAsync(Guid Id, CancellationToken cancellationToken);
        public Task<int> GetSubscriptionCount(Guid id, CancellationToken cancellationToken);
    }
}
