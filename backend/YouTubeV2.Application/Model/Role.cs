using Microsoft.AspNetCore.Identity;

namespace YouTubeV2.Application.Model
{
    public class Role : IdentityRole
    {
        public static readonly string User = "USER";
        public static readonly string Creator = "CREATOR";
        public static readonly string Admin = "ADMIN";

        public Role(string roleName) : base(roleName)
        {
            NormalizedName = roleName.ToUpper();
        }

        public Role() { }
    }
}
