using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using YouTubeV2.Api.Attributes;
using YouTubeV2.Application.DTO.SubscriptionDTOS;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Services;

namespace YouTubeV2.Api.Controllers
{
    [ApiController]
    [Route("api/subscriptions")]
    [Roles(Role.Simple, Role.Creator, Role.Administrator)]
    public class SubscriptionsController : IdentityControllerBase
	{
		private readonly ISubscriptionService _subscriptionsService;

		public SubscriptionsController(ISubscriptionService subscriptionsService)
		{
			_subscriptionsService = subscriptionsService;
		}

        [HttpGet]
		public async Task<ActionResult<UserSubscriptionListDto>> GetSubscriptionsAsync([FromQuery] Guid? id, CancellationToken cancellationToken)
        {
            if (id is null) id = new Guid(GetUserId()!);

            return Ok(await _subscriptionsService.GetSubscriptionsAsync(id.ToString()!, cancellationToken));
        }        

        [HttpPost]
        public async Task<IActionResult> PostSubscriptionsAsync([FromQuery][Required] Guid creatorId, CancellationToken cancellationToken)
        {
            string? subscriberId = GetUserId();
            if (subscriberId is null) return Forbid();

            await _subscriptionsService.AddSubscriptionAsync(creatorId.ToString(), subscriberId, cancellationToken);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteSubscriptionsAsync([FromQuery][Required] Guid subId, CancellationToken cancellationToken)
        {
            string? subscriberId = GetUserId();
            if (subscriberId is null) return Forbid();

            await _subscriptionsService.DeleteSubscriptionAsync(subId.ToString(), subscriberId, cancellationToken);
            return Ok();
        }
    }
}