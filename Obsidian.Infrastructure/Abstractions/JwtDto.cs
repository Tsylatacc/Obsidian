namespace Obsidian.Infrastructure.Abstractions
{
    public sealed record JwtDto(
        string Token,
        DateTimeOffset ExpiresAt);
}
