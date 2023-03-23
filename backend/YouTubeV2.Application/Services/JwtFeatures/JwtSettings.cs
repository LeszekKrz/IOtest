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
            ValidAudience = configurationSection["validAudience"];
            ExpiresInDays = Convert.ToDouble(configurationSection["expiresInDays"]);
        }
        public JwtSettings()
        {
            SecurityKey = Encoding.UTF8.GetBytes("65776657776546547676547654654776545746547654765465I");
            ValidIssuer = "d8768665476547654765476547654765476547876867ont";
            ValidAudience = "k6878768765476547654765465477654765467876now";
            ExpiresInDays = 0.123;
        }
    }
}