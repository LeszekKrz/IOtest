using FluentValidation;
using Microsoft.EntityFrameworkCore;
using YouTubeV2.Application.DTO;
using YouTubeV2.Application.Services.AzureServices.BlobServices;

namespace YouTubeV2.Application.Services
{
    public class SubscriptionsService
    {
        private readonly IBlobImageService _blobImageService;
        private readonly YTContext _context;
        public SubscriptionsService(IBlobImageService blobImageService, YTContext context)
        {
            _blobImageService = blobImageService;
            _context = context;
        }

        public async Task<UserSubscriptionListDTO> GetSubscriptionsAsync(Guid Id, CancellationToken cancellationToken)
        {
            return new UserSubscriptionListDTO( await _context.Subscriptions.
                Where(s => s.SubscriberId == Id.ToString()).
                Select(s => new SubscriptionDTO(new Guid(s.SubscribeeId), _blobImageService.GetProfilePicture(s.Subscribee.Id), s.Subscribee.UserName)).
                ToListAsync(cancellationToken));
        }

        public async Task<int> GetSubscriptionCount(Guid id, CancellationToken cancellationToken)
        {
            var userId = id.ToString();

            return await _context.Subscriptions.CountAsync(s => s.SubscribeeId == userId, cancellationToken);
        }
    }
}