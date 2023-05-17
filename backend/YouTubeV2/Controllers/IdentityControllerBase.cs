using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace YouTubeV2.Api.Controllers
{
    public abstract class IdentityControllerBase : ControllerBase
    {
        protected string? GetUserId() => User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

        protected string? GetUserRole() => User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role)?.Value;
    }
}
