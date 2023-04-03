using Microsoft.AspNetCore.Identity;

namespace YouTubeV2.Application.Model
{
    public class Role : IdentityRole
    {
        public const string Simple = "Simple";
        public const string Creator = "Creator";
        public const string Administrator = "Administrator";

        public Role(string roleName) : base(roleName)
        {
            NormalizedName = roleName.ToUpper();
        }

        public Role() { }
    }
}
