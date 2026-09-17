namespace Obsidian.Infrastructure.Abstractions
{
    public interface IJwtService
    {
        JwtDto GenerateBearerToken(Guid userId, Guid tenantId, string email);
    }
}
