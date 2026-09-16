using Microsoft.EntityFrameworkCore;
using Obsidian.Domain.Entities;
using Obsidian.Infrastructure.Abstractions;
using Obsidian.Infrastructure.Persistence;
using Obsidian.Infrastructure.Security;

namespace Obsidian.Application.Features
{
    public class SignInHandler
    {
        public static async Task<SignInResult> Handle(
             SignInCommand command,
             ObsidianDbContext db,
             IJwtService jwtService,
             CancellationToken cancellationToken)
        {
            User user = await db.Users
                .IgnoreQueryFilters()
                .SingleOrDefaultAsync(x => x.Email == command.Email, cancellationToken)
                ?? throw new InvalidOperationException("Invalid credentials.");

            if (!SecurityService.VerifyPassword(command.Password, user.PasswordHash))
                throw new InvalidOperationException("Invalid credentials.");

            JwtDto jwtDto = jwtService.GenerateBearerToken(user.Id, user.Email);
            return new SignInResult(
                jwtDto.Token,
                jwtDto.ExpiresAt);
        }
    }
    public sealed record SignInCommand(
        string Email,
        string Password
    );
    public sealed record SignInResult(
        string Token,
        DateTimeOffset ExpiresAt
    );
}
