using Microsoft.AspNetCore.Authorization;

namespace YouTubeV2.Api.Attributes
{
    public class RolesAttribute : AuthorizeAttribute
    {
        public RolesAttribute(params string[] roles) 
        {
            Roles = string.Join(",", roles);
        }
    }
}
