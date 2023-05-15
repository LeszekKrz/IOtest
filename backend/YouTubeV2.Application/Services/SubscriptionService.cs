using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YouTubeV2.Application.DTO.SubscriptionDTOS;
using YouTubeV2.Application.Exceptions;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Services.BlobServices;

namespace YouTubeV2.Application.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IBlobImageService _blobImageService;
        private readonly YTContext _context;
        private readonly UserManager<User> _userManager;

        public SubscriptionService(IBlobImageService blobImageService, YTContext context, UserManager<User> userManager)
        {
            _blobImageService = blobImageService;
            _context = context;
            _userManager = userManager;
        }

        public async Task<UserSubscriptionListDto> GetSubscriptionsAsync(string id, CancellationToken cancellationToken = default) =>
             new UserSubscriptionListDto(await _context
                 .Subscriptions
                 .Include(subscribtion => subscribtion.Subscribee)
                 .Where(s => s.SubscriberId == id)
                 .Select(s => new SubscriptionDto(s.SubscribeeId, _blobImageService.GetProfilePictureUrl(s.Subscribee.Id), s.Subscribee.UserName!))
                 .ToArrayAsync(cancellationToken));

        public async Task<int> GetSubscriptionCountAsync(string id, CancellationToken cancellationToken = default) =>
            await _context.Subscriptions.CountAsync(s => s.SubscribeeId == id, cancellationToken);

        public async Task AddSubscriptionAsync(string subscribeeId, string subscriberId, CancellationToken cancellationToken = default)
        {
            var subscribee = await _userManager.FindByIdAsync(subscribeeId)
                ?? throw new NotFoundException($"User with id {subscribeeId} you want to subscribe not found");

            var subscriber = await _userManager.FindByIdAsync(subscriberId)
                ?? throw new NotFoundException($"User with id {subscriberId} that is subscribing not found");


            if (await _context
                .Subscriptions
                .AnyAsync(subscription => subscription.SubscriberId == subscriberId && subscription.SubscribeeId == subscribeeId, cancellationToken))
                throw new BadRequestException($"User with id {subscriberId} is already subscribed to a user with id {subscribeeId}");

            Subscription subRequest = new(subscribee, subscriber);
            await _context.Subscriptions.AddAsync(subRequest, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteSubscriptionAsync(string subscribeeId, string subscriberId, CancellationToken cancellationToken = default)
        {
            Subscription subscription = await _context
                .Subscriptions
                .FirstOrDefaultAsync(subscription => subscription.SubscribeeId == subscribeeId && subscription.SubscriberId == subscriberId, cancellationToken)
                ?? throw new NotFoundException($"Subscription with subscriber id {subscriberId} and subscribee id {subscribeeId} not found");

            _context.Subscriptions.Remove(subscription);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAllSubscriptionsRelatedToUserAsync(User user, CancellationToken cancellationToken = default)
        {
            var subscriptions = await _context.Subscriptions.Where(
                subscription => subscription.Subscriber == user || subscription.Subscribee == user).ToListAsync(cancellationToken);

            if (subscriptions.Count == 0)
                return;

            _context.RemoveRange(subscriptions);
                
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAllSubscriptionsWhereUserIsSubscribeeAsync(User user, CancellationToken cancellationToken = default)
        {
            var subscriptions = await _context.Subscriptions.Where(subscription => subscription.Subscribee == user).ToListAsync(cancellationToken);

            if (subscriptions.Count == 0)
                return;

            _context.RemoveRange(subscriptions);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}