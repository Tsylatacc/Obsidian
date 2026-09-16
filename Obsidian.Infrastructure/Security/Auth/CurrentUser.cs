using Microsoft.AspNetCore.Http;
using Obsidian.Infrastructure.Abstractions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Obsidian.Infrastructure.Security.Auth
{
    public sealed class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _context;

        public CurrentUser(
            IHttpContextAccessor context)
        {
            _context = context;
        }

        private ClaimsPrincipal User =>
            _context.HttpContext?.User
            ?? throw new UnauthorizedAccessException("Unauthenticated user.");

        public Guid UserId =>
            Guid.Parse(
                User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                ?? throw new UnauthorizedAccessException("UserId claim not found."));
        
        public Guid TenantId =>
            Guid.Parse(
                User.FindFirst(JwtCustomClaims.TenantId)?.Value
                ?? throw new UnauthorizedAccessException("UserId claim not found."));

        public string Email =>
            User.FindFirst(JwtRegisteredClaimNames.Email)?.Value
            ?? throw new UnauthorizedAccessException("Email claim not found.");
    }
}