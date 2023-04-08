using System.Security.Claims;
using YouTubeV2.Application.Model;

namespace YouTubeV2.Api.Tests.Providers
{
    public class ClaimsProvider
    {
        public ICollection<Claim> Claims { get; } = new List<Claim>();

        internal static ClaimsProvider WithSimpleAccess()
        {
            ClaimsProvider claimsProvider = new();
            claimsProvider.Claims.Add(new(ClaimTypes.Role, Role.Simple));
            return claimsProvider;
        }

        internal static ClaimsProvider WithSimpleAccessAndUserId(string userId)
        {
            ClaimsProvider claimsProvider = new();
            claimsProvider.Claims.Add(new(ClaimTypes.Role, Role.Simple));
            claimsProvider.Claims.Add(new(ClaimTypes.NameIdentifier, userId));
            return claimsProvider;
        }

        internal static ClaimsProvider WithCreatorAccessAndUserId(string userId)
        {
            ClaimsProvider claimsProvider = new();
            claimsProvider.Claims.Add(new (ClaimTypes.Role, Role.Creator));
            claimsProvider.Claims.Add(new (ClaimTypes.NameIdentifier, userId));
            return claimsProvider;
        }
}
}
