using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Obsidian.Infrastructure.Abstractions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace Obsidian.Infrastructure.Security.Auth
{
    public sealed class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public JwtDto GenerateBearerToken(Guid userId, string email)
        {
            Claim[] claims =
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            SymmetricSecurityKey key = new(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Bearer:Key"]!)
            );

            SigningCredentials credentials = new(
                key,
                SecurityAlgorithms.HmacSha256
            );

            DateTimeOffset expiresAt = DateTimeOffset.UtcNow.AddMinutes(
                int.Parse(_configuration["Jwt:Bearer:ExpiresInMinutes"]!));

            JwtSecurityToken token = new(
                issuer: _configuration["Jwt:Bearer:Issuer"],
                audience: _configuration["Jwt:Bearer:Audience"],
                claims: claims,
                expires: expiresAt.UtcDateTime,
                signingCredentials: credentials
            );

            return new JwtDto(
                new JwtSecurityTokenHandler().WriteToken(token),
                expiresAt.UtcDateTime);
        }
    }
}