using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using YouTubeV2.Application.DTO;
using YouTubeV2.Application.Services;

namespace YouTubeV2.Api.Controllers
{
	[ApiController]
	public class SubscriptionsController : ControllerBase
	{
		private readonly SubscriptionsService _subscriptionsService;

		public SubscriptionsController(SubscriptionsService subscriptionsService)
		{
			_subscriptionsService = subscriptionsService;
		}

		[HttpGet("subscriptions")]
		public async Task<ActionResult<UserSubscriptionListDTO>> GetSubscriptionsAsync([FromQuery][Required] Guid id, CancellationToken cancellationToken)
		{
			return Ok((await _subscriptionsService.GetSubscriptionsAsync(id, cancellationToken)));
		}
	}
}