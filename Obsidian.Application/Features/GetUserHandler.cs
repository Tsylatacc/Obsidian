using Microsoft.EntityFrameworkCore;
using Obsidian.Infrastructure.Abstractions;
using Obsidian.Infrastructure.Persistence;

namespace Obsidian.Application.Features;

public static class GetUserHandler
{
    public static async Task<GetUserResponse> Handle(
        GetUserCommand command,
        ObsidianDbContext db,
        ICurrentUser currentUser,
        CancellationToken cancellationToken)
    {
        GetUserResult user = await db.Users
            .AsNoTracking()
            .Where(x => x.Id == currentUser.UserId)
            .Select(x => new GetUserResult(
                x.Id,
                x.Name,
                x.Email,
                x.CreatedAt))
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw new KeyNotFoundException(
                $"User {currentUser.UserId} was not found.");

        return new GetUserResponse(user);
    }
}

public sealed record GetUserCommand();

public sealed record GetUserResponse(
    GetUserResult User
);

public sealed record GetUserResult(
    Guid Id,
    string Name,
    string Email,
    DateTimeOffset CreatedAt
);
