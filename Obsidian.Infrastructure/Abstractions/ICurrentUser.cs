namespace Obsidian.Infrastructure.Abstractions
{
    public interface ICurrentUser
    {
        public Guid UserId { get; }
        public Guid TenantId { get; }
        public string Email { get; }
    }
}
