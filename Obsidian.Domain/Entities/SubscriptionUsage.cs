using JasperFx.MultiTenancy;

namespace Obsidian.Domain.Entities
{
    public class SubscriptionUsage : ITenanted
    {
        public Guid Id { get; private set; }

        public Guid SubscriptionId { get; private set; }

        public int MessagedUsed { get; private set; }
        public int ChannelsUsed { get; private set; }

        public string? TenantId { get; set; }
        public DateTimeOffset PeriodStartedAt { get; private set; }
        public DateTimeOffset PeriodExpiresAt { get; private set; }

        private SubscriptionUsage() { } // EF Core

        private SubscriptionUsage(
            Guid subscriptionId,
            DateTimeOffset periodStartedAt,
            DateTimeOffset periodExpiresAt)
        {
            Id = Guid.NewGuid();
            SubscriptionId = subscriptionId;
            PeriodStartedAt = periodStartedAt;
            PeriodExpiresAt = periodExpiresAt;
        }

        public static SubscriptionUsage Create(
            Subscription subscription)
        {
            ArgumentNullException.ThrowIfNull(subscription);

            return new SubscriptionUsage(
                subscription.Id,
                subscription.StartedAt,
                subscription.ExpiresAt);
        }
    }
}
