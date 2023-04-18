using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using YouTubeV2.Api.Attributes;
using YouTubeV2.Application.DTO.SubscribtionDTOS;
using YouTubeV2.Application.Exceptions;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Services;
using YouTubeV2.Application.Services.JwtFeatures;

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
		public async Task<ActionResult<UserSubscribtionListDto>> GetSubscriptionsAsync([FromQuery][Required] Guid id, CancellationToken cancellationToken)
		{
			return Ok((await _subscriptionsService.GetSubscriptionsAsync(id, cancellationToken)));
        }

        [Roles(Role.Simple, Role.Creator, Role.Administrator)]
        [HttpPost]
        public async Task<IActionResult> PostSubscriptionsAsync([FromQuery][Required] Guid subscribeeGuid, CancellationToken cancellationToken)
        {
            string jwtToken = HttpContext.Request.Headers["Authorization"].ToString();

            Guid subscriberGuid = JwtHandler.ExtractUserGuidFromToken(jwtToken);

            await _subscriptionsService.PostSubscriptionsAsync(subscribeeGuid, subscriberGuid, cancellationToken);

            return Ok();
        }

        [Roles(Role.Simple, Role.Creator, Role.Administrator)]
        [HttpDelete]
        public async Task<IActionResult> DeleteSubscriptionsAsync([FromQuery][Required] Guid subscribeeGuid, CancellationToken cancellationToken)
        {
            string jwtToken = HttpContext.Request.Headers["Authorization"].ToString();

            Guid subscriberGuid = JwtHandler.ExtractUserGuidFromToken(jwtToken);

            await _subscriptionsService.DeleteSubscriptionsAsync(subscribeeGuid, subscriberGuid, cancellationToken);

            return Ok();
        }
    }
}