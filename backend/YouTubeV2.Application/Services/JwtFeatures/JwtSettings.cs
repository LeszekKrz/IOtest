using Microsoft.Extensions.Configuration;
using System.Text;

namespace YouTubeV2.Application.Services.JwtFeatures
{
    public class JwtSettings
    {
        public byte[] SecurityKey { get; }
        public string ValidIssuer { get; }
        public string ValidAudience { get; }
        internal double ExpiresInDays { get; }

        public JwtSettings(IConfiguration configurationSection)
        {
            SecurityKey = Encoding.UTF8.GetBytes(configurationSection["securityKey"]);
            ValidIssuer = configurationSection["validIssuer"];
        }
    }
}