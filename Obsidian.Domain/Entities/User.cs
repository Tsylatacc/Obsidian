using JasperFx.MultiTenancy;

namespace Obsidian.Domain.Entities
{
    public class User : ITenanted
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = default!;
        public string Email { get; private set; } = default!;
        public string PasswordHash { get; private set; } = default!;
        public bool IsAdmin { get; private set; }

        public Guid SubscriptionId { get; private set; }
        public Subscription Subscription { get; private set; } = default!;

        private readonly List<Channel> _channels = [];
        public IReadOnlyCollection<Channel> Channels => _channels.AsReadOnly();

        public string? TenantId { get; set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset UpdatedAt { get; private set; }

        private User() { } // EF Core

        private User(
            string email,
            string passwordHash,
            Guid subscriptionId,
            bool isAdmin = false)
        {
            Id = Guid.NewGuid();
            Name = email;
            Email = email;
            PasswordHash = passwordHash;
            IsAdmin = isAdmin;

            SubscriptionId = subscriptionId;
        }

        public static User Create(
            string email,
            string passwordHash,
            Guid subscriptionId,
            bool isAdmin = false)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException(
                    "Argument 'email' cannot be empty.",
                    nameof(email));

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException(
                    "Argument 'passwordHash' cannot be empty.",
                    nameof(passwordHash));

            if (subscriptionId == Guid.Empty)
                throw new ArgumentException(
                    "Argument 'subscriptionId' cannot be empty.",
                    nameof(subscriptionId));

            return new User(
                email,
                passwordHash,
                subscriptionId,
                isAdmin);
        }
    }
}
