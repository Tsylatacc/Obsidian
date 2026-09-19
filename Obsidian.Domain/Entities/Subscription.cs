using JasperFx.MultiTenancy;
using System.Linq;
using Obsidian.Domain.Enums;

namespace Obsidian.Domain.Entities
{
    public class Subscription : ITenanted
    {
        public Guid Id { get; private set; }
        public string Identifier { get; private set; } = default!;

        public Guid PlanId { get; private set; }
        public SubscriptionPlan Plan { get; private set; } = default!;

        private readonly List<SubscriptionUsage> _subscriptionUsages = [];
        public IReadOnlyCollection<SubscriptionUsage> SubscriptionUsages => _subscriptionUsages.AsReadOnly();

        public SubscriptionStatus Status { get; private set; }

        private readonly List<Payment> _payments = [];
        public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();

        public string? TenantId { get; set; }
        public DateTimeOffset StartedAt { get; private set; }
        public DateTimeOffset ExpiresAt { get; private set; }
        public DateTimeOffset UpdatedAt { get; private set; }

        private Subscription() { } // EF Core

        private Subscription(
            SubscriptionPlan subscriptionPlan)
        {
            Id = Guid.NewGuid();
            Identifier = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();

            PlanId = subscriptionPlan.Id;
            Plan = subscriptionPlan;

            Status = SubscriptionStatus.PendingActivation;

            StartedAt = DateTimeOffset.UtcNow;
            ExpiresAt = StartedAt.Add(subscriptionPlan.Duration);
            UpdatedAt = StartedAt;
        }

        public static Subscription Create(
            SubscriptionPlan subscriptionPlan)
        {
            ArgumentNullException.ThrowIfNull(subscriptionPlan);

            Subscription subscription = new(
                subscriptionPlan);

            return subscription;
        }

        public void Activate()
        {
            if (Status == SubscriptionStatus.Active)
                return;

            Status = SubscriptionStatus.Active;
            UpdatedAt = DateTimeOffset.UtcNow;
        }

        public void AddUsage()
        {
            if (_subscriptionUsages.LastOrDefault() is { } lastUsage &&
                lastUsage.PeriodExpiresAt > DateTimeOffset.UtcNow)
            {
                throw new InvalidOperationException(
                    "The most recent use of the subscription is still within the expiration period.");
            }

            SubscriptionUsage subscriptionUsage = SubscriptionUsage.Create(this);
            _subscriptionUsages.Add(subscriptionUsage);
        }

        public void EnsureIsValid()
        {
            if (!Plan.IsActive)
                throw new InvalidOperationException($"Subscription plan {PlanId} is not active.");
            
            if (Status != SubscriptionStatus.Active)
                throw new InvalidOperationException($"Subscription {Id} is not active.");
        }
    }
}
