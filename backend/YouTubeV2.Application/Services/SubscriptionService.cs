using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using YouTubeV2.Application.DTO;
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
        public async Task PostSubscriptionsAsync(Guid subscribeeGuid, string? subscriberToken, CancellationToken cancellationToken)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(subscriberToken))
            {
                throw new BadRequestException();
            }
            string? subscriberId = handler.ReadJwtToken(subscriberToken).Claims.FirstOrDefault(c => c.Type == "NameIdentifier")?.Value;
            if (subscriberId.IsNullOrEmpty())
            {
                throw new BadRequestException();
            }
            var subscribee = await _userManager.FindByIdAsync(subscribeeGuid.ToString());
            var subscriber = await _userManager.FindByIdAsync(subscriberId);
            if (subscribee == null || subscriber == null)
            {
                throw new BadRequestException();
            }

            Subscription subRequest = new()
            {
                SubscribeeId = subscribee.Id,
                Subscribee = subscribee,
                SubscriberId = subscriber.Id,
                Subscriber = subscriber
            };

            var existingCopies = _context.Subscriptions.
                Where(s => s.SubscriberId == subRequest.SubscriberId && s.SubscribeeId == subRequest.SubscribeeId).
                ToArray();
            if (existingCopies.Length > 0)
            {
                throw new BadRequestException();
            }
            await _context.Subscriptions.AddAsync(subRequest, cancellationToken);

            _context.SaveChanges();
        }

        public async Task DeleteSubscriptionsAsync(Guid subscribeeGuid, string? subscriberToken, CancellationToken cancellationToken)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(subscriberToken))
            {
                throw new BadRequestException();
            }
            string? subscriberId = handler.ReadJwtToken(subscriberToken).Claims.FirstOrDefault(c => c.Type == "NameIdentifier")?.Value;
            if (subscriberId.IsNullOrEmpty())
            {
                throw new BadRequestException();
            }
            var subs = await _context.Subscriptions.Where(s => s.SubscribeeId == subscribeeGuid.ToString() && s.SubscriberId == subscriberId)
                .ToArrayAsync(cancellationToken);
            if (subs.Length == 0)
            {
                throw new BadRequestException();
            }

            _context.Subscriptions.Remove(subs[0]);
            _context.SaveChanges();
        }
    }
}