using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using YouTubeV2.Api.Attributes;
using YouTubeV2.Application.DTO;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Services;

namespace YouTubeV2.Api.Controllers
{
	[ApiController]
	public class SubscriptionsController : ControllerBase
	{
		private readonly ISubscriptionService _subscriptionsService;

		public SubscriptionsController(ISubscriptionService subscriptionsService)
		{
			_subscriptionsService = subscriptionsService;
		}
        [HttpGet("subscriptions")]
		public async Task<ActionResult<UserSubscriptionListDTO>> GetSubscriptionsAsync([FromQuery][Required] Guid id, CancellationToken cancellationToken)
		{
			return Ok((await _subscriptionsService.GetSubscriptionsAsync(id, cancellationToken)));
        }
        [Roles(Role.Simple, Role.Creator, Role.Administrator)]
        [HttpPost("subscriptions")]
        public async Task<IActionResult> PostSubscriptionsAsync([FromQuery][Required] Guid id, CancellationToken cancellationToken)
        {
            string jwtToken = HttpContext.Request.Headers["Authorization"].ToString();

            await _subscriptionsService.PostSubscriptionsAsync(id, jwtToken, cancellationToken);

            return Ok();
        }
        [Roles(Role.Simple, Role.Creator, Role.Administrator)]
        [HttpDelete("subscriptions")]
        public async Task<IActionResult> DeleteSubscriptionsAsync([FromQuery][Required] Guid id, CancellationToken cancellationToken)
        {
            string jwtToken = HttpContext.Request.Headers["Authorization"].ToString();

            await _subscriptionsService.DeleteSubscriptionsAsync(id, jwtToken, cancellationToken);

            return Ok();
        }
    }
}