using Microsoft.AspNetCore.Identity;

namespace YouTubeV2.Application.Model
{
    public class Role : IdentityRole
    {
        public static readonly string Simple = "Simple";
        public static readonly string Creator = "Creator";
        public static readonly string Administrator = "Administrator";

        public Role(string roleName) : base(roleName)
        {
            NormalizedName = roleName.ToUpper();
        }

        public Role() { }
    }
}
