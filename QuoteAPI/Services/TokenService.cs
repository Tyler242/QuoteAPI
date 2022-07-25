using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using QuoteAPI.Helpers;
using QuoteAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QuoteAPI.Services
{
    public class TokenService : ITokenService
    {
        private const double EXPIRY_DURATION_MINUTES = 30;
        private readonly IOptions<JWTAuthSettings> _jwtTokenSettings;

        public TokenService(IOptions<JWTAuthSettings> jwtTokenSettings)
        {
            _jwtTokenSettings = jwtTokenSettings;
        }

        public string BuildToken(UserDTO user)
        {
            List<Claim>? claims = new()
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Role, user.Role!)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes((_jwtTokenSettings.Value.Key ?? Environment.GetEnvironmentVariable("JWTKey"))!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new JwtSecurityToken(_jwtTokenSettings.Value.Issuer ?? Environment.GetEnvironmentVariable("JWTIssuer"),
                _jwtTokenSettings.Value.Issuer ?? Environment.GetEnvironmentVariable("JWTIssuer"), claims,
                expires: DateTime.Now.AddMinutes(EXPIRY_DURATION_MINUTES), signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        public bool IsTokenValid(string token)
        {
            var mySecret = Encoding.UTF8.GetBytes((_jwtTokenSettings.Value.Key ?? Environment.GetEnvironmentVariable("JWTKey"))!);
            var mySecurityKey = new SymmetricSecurityKey(mySecret);
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _jwtTokenSettings.Value.Issuer ?? Environment.GetEnvironmentVariable("JWTIssuer"),
                    ValidAudience = _jwtTokenSettings.Value.Issuer ?? Environment.GetEnvironmentVariable("JWTIssuer"),
                    IssuerSigningKey = mySecurityKey,
                }, out SecurityToken validatedToken);
            } 
            catch
            {
                return false;
            }
            return true;
        }
    }
}
