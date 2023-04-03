using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using YouTubeV2.Application.DTO;
using YouTubeV2.Application.Exceptions;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Services.AzureServices.BlobServices;
using YouTubeV2.Application.Validator;

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
                Where(s => new Guid(s.SubscriberId) == Id).
                Select(s => new SubscriptionDTO(new Guid(s.SubscribeeId), _blobImageService.GetProfilePicture(s.Subscribee.Id), s.Subscribee.UserName)).
                ToListAsync(cancellationToken));
        }
    }
}