using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using YouTubeV2.Api.Attributes;
using YouTubeV2.Application.Model;
using YouTubeV2.Application.Services;

namespace YouTubeV2.Api.Controllers
{
    [ApiController]
    [Route("donate")]
    public class DonationController : IdentityControllerBase
    {
        private readonly IDonationService _donationService;

        public DonationController(IDonationService donationService) 
        {
            _donationService = donationService;
        }

        [HttpPost("send")]
        [Roles(Role.Simple, Role.Administrator)]
        public async Task<ActionResult> SendDontaionAsync([FromQuery][Required] Guid id, [FromQuery][Required] decimal amount)
        {
            string? senderId = GetUserId();
            if (senderId is null) return BadRequest();

            await _donationService.SendDonationAsync(senderId, id.ToString(), amount);

            return Ok();
        }

        [HttpPost("withdraw")]
        [Roles(Role.Simple, Role.Creator)]
        public async Task<ActionResult> WithdrawMoneyAsync([FromQuery][Required] decimal amount)
        {
            string? withdrawerId = GetUserId();
            if (withdrawerId is null) return BadRequest();

            await _donationService.WithdrawMoneyAsync(withdrawerId, amount);

            return Ok();
        }
    }
}
