using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using CouponHub.DataAccess.Models;

namespace CouponHub.Business.Services
{
    public class AuthService
    {
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateJwtToken(User user)
        {
            var jwtSection = _configuration.GetSection("Jwt");
            var key = jwtSection.GetValue<string>("Key") ?? "default_jwt_key_change_in_production";
            var issuer = jwtSection.GetValue<string>("Issuer") ?? "couponhub";
            var audience = jwtSection.GetValue<string>("Audience") ?? "couponhub_clients";
            var expiryMinutes = jwtSection.GetValue<int>("ExpiryMinutes");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FirstName ?? ""),
                new Claim(ClaimTypes.Role, user.Role ?? "Customer"),
                new Claim("id", user.Id.ToString()),
                new Claim("name", user.FirstName ?? ""),
                new Claim("role", user.Role ?? "Customer"),
                new Claim("email", user.Email ?? ""),
                new Claim("mobileNumber", user.MobileNumber ?? ""),
                new Claim("isActive", user.IsActive.ToString().ToLower()),
                new Claim("lastName", user.LastName ?? ""),
                new Claim("authProvider", "internal")
            };

            if (user.ServiceCenterId.HasValue)
            {
                claims.Add(new Claim("serviceCenterId", user.ServiceCenterId.Value.ToString()));
            }

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
