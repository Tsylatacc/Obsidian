using JasperFx.MultiTenancy;
using Obsidian.Domain.Enums;

namespace Obsidian.Domain.Entities
{
    public class Payment : ITenanted
    {
        public Guid Id { get; private set; }

        public Guid SubscriptionId { get; private set; }
        public Subscription Subscription { get; private set; } = default!;

        public decimal Amount { get; private set; }
        public string? ExternalId { get; private set; }
        public string? CheckoutUrl { get; private set; }
        public PaymentStatus Status { get; private set; }

        public string? TenantId { get; set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset? PaidAt { get; private set; }

        private Payment() { } // EF Core

        private Payment(
            Guid subscriptionId,
            decimal amount)
        {
            Id = Guid.NewGuid();
            SubscriptionId = subscriptionId;
            Amount = amount;
            Status = PaymentStatus.Pending;
            CreatedAt = DateTimeOffset.UtcNow;
        }

        public static Payment Create(
            Guid subscriptionId,
            decimal amount)
        {
            if (subscriptionId == Guid.Empty)
                throw new ArgumentException(
                    "Argument 'subscriptionId' cannot be empty.",
                    nameof(subscriptionId));

            if (amount <= 0)
                throw new ArgumentException(
                    "Argument 'amount' must be greater than zero.",
                    nameof(amount));

            return new Payment(subscriptionId, amount);
        }

        public void MarkAsPaid()
        {
            Status = PaymentStatus.Paid;
            PaidAt = DateTimeOffset.UtcNow;
        }
    }
}
