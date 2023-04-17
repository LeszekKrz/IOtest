using YouTubeV2.Application.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace YouTubeV2.Application.Services.JwtFeatures
{
    public class JwtHandler
    {
        private readonly JwtSettings _jwtSettings;
        private readonly UserManager<User> _userManager;

        public JwtHandler(JwtSettings jwtSettings, UserManager<User> userManager)
        {
            _jwtSettings = jwtSettings;
            _userManager = userManager;
        }

        public async Task<string> GenerateTokenAsync(User user)
        {
            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaimsAsync(user);
            var tokenOptions = GenerateJwtSecurityToken(signingCredentials, claims);
            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        private SigningCredentials GetSigningCredentials()
        {
            var symmetricSecurityKey = new SymmetricSecurityKey(_jwtSettings.SecurityKey);
            return new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
        }

        private async Task<ICollection<Claim>> GetClaimsAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            IList<string> roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            return claims;
        }

        private JwtSecurityToken GenerateJwtSecurityToken(SigningCredentials signingCredentials, ICollection<Claim> claims)
        {
            return new JwtSecurityToken(
                issuer: _jwtSettings.ValidIssuer,
                audience: _jwtSettings.ValidAudience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwtSettings.ExpiresInDays),
                signingCredentials: signingCredentials);
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            JwtSecurityTokenHandler jwtSecurityTokenHandler = new();
            TokenValidationParameters tokenValidationParameters = new()
            {
                ValidIssuer = _jwtSettings.ValidIssuer,
                ValidAudience = _jwtSettings.ValidAudience,
                IssuerSigningKey = new SymmetricSecurityKey(_jwtSettings.SecurityKey),
            };

            return jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out var _);
        }
    }
}