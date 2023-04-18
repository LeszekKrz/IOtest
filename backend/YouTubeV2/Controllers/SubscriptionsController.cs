using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using YouTubeV2.Api.Attributes;
using YouTubeV2.Application.DTO;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Services;

namespace YouTubeV2.Api.Controllers
{
    [ApiController]
    [Route("subscriptions")]
    public class SubscriptionsController : ControllerBase
	{
		private readonly ISubscriptionService _subscriptionsService;

		public SubscriptionsController(ISubscriptionService subscriptionsService)
		{
			_subscriptionsService = subscriptionsService;
		}

        [Roles(Role.Simple, Role.Creator, Role.Administrator)]
        [HttpGet]
		public async Task<ActionResult<UserSubscriptionListDTO>> GetSubscriptionsAsync([FromQuery][Required] Guid id, CancellationToken cancellationToken) =>
            Ok(await _subscriptionsService.GetSubscriptionsAsync(id.ToString(), cancellationToken));

        [Roles(Role.Simple, Role.Creator, Role.Administrator)]
        [HttpPost]
        public async Task<IActionResult> PostSubscriptionsAsync([FromQuery][Required] Guid subId, CancellationToken cancellationToken)
        {
            string subscriberId = GetUserId();
            await _subscriptionsService.AddSubscriptionAsync(subId.ToString(), subscriberId, cancellationToken);
            return Ok();
        }

        [Roles(Role.Simple, Role.Creator, Role.Administrator)]
        [HttpDelete]
        public async Task<IActionResult> DeleteSubscriptionsAsync([FromQuery][Required] Guid subId, CancellationToken cancellationToken)
        {
            string subscriberId = GetUserId();
            await _subscriptionsService.DeleteSubscriptionAsync(subId.ToString(), subscriberId, cancellationToken);
            return Ok();
        }

        private string GetUserId() => User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
    }
}