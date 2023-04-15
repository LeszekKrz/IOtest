using System.Security.Claims;
using YouTubeV2.Application.Model;

namespace YouTubeV2.Api.Tests.Providers
{
    public class ClaimsProvider
    {
        public ICollection<Claim> Claims { get; } = new List<Claim>();

        internal static ClaimsProvider WithRoleAccess(string role)
        {
            ClaimsProvider claimsProvider = new();
            claimsProvider.Claims.Add(new(ClaimTypes.Role, role));
            return claimsProvider;
        }

        internal static ClaimsProvider WithRoleAccessAndUserId(string role, string userId)
        {
            ClaimsProvider claimsProvider = new();
            claimsProvider.Claims.Add(new(ClaimTypes.Role, role));
            claimsProvider.Claims.Add(new(ClaimTypes.NameIdentifier, userId));
            return claimsProvider;
        }
}
}
