using Microsoft.AspNetCore.Identity;

namespace YouTubeV2.Application.Model
{
    public class UserRole : IdentityUserRole<string>
    {
        public User User { get; private set; }
        public Role Role { get; private set; }

        public UserRole() { }
    }
}
