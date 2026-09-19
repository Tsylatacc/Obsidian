using JasperFx.MultiTenancy;

namespace Obsidian.Domain.Entities
{
    public class SubscriptionUsage : ITenanted
    {
        public Guid Id { get; private set; }

        public Guid SubscriptionId { get; private set; }
        public Subscription Subscription { get; private set; } = default!;

        public int CampaignsUsed { get; private set; }
        public int MessagesUsed { get; private set; }
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

        public void EnsureIsValid(
            int itemsCount,
            int phoneNumbersCount)
        {
            if (PeriodExpiresAt <= DateTimeOffset.UtcNow)
                throw new InvalidOperationException(
                    "The subscription usage period has expired.");

            if (!Subscription.SubscriptionPlan.IsActive)
                throw new InvalidOperationException(
                    "Subscription is not active.");

            if (itemsCount <= 0)
                throw new ArgumentException(
                    "At least one item is required.",
                    nameof(itemsCount));

            if (phoneNumbersCount <= 0)
                throw new ArgumentException(
                    "At least one phone number is required.",
                    nameof(phoneNumbersCount));

            if (phoneNumbersCount > Subscription.SubscriptionPlan.RecipientPerCampaignLimit)
                throw new InvalidOperationException(
                    "The subscription recipient limit has been exceeded.");

            if (CampaignsUsed >= Subscription.SubscriptionPlan.CampaignLimit)
                throw new InvalidOperationException(
                    "The subscription campaign limit has been exceeded.");

            if (MessagesUsed + itemsCount * phoneNumbersCount > Subscription.SubscriptionPlan.MessageLimit)
                throw new InvalidOperationException(
                    "The subscription message limit has been exceeded.");
        }

        public void AddUsage(int itemsCount, int phoneNumbersCount)
        {
            if (CampaignsUsed >= Subscription.SubscriptionPlan.CampaignLimit)
                throw new InvalidOperationException(
                    "The subscription campaign limit has been exceeded.");

            if (MessagesUsed + itemsCount * phoneNumbersCount > Subscription.SubscriptionPlan.MessageLimit)
                throw new InvalidOperationException(
                    "The subscription message limit has been exceeded.");

            CampaignsUsed++;
            MessagesUsed += itemsCount * phoneNumbersCount;
        }

        public int CampaignsRemaining => 
            Subscription.SubscriptionPlan.CampaignLimit - CampaignsUsed;
        public int MessagesRemaining => 
            Subscription.SubscriptionPlan.MessageLimit - MessagesUsed;
        public int ChannelsRemaining => 
            Subscription.SubscriptionPlan.ChannelLimit - ChannelsUsed;
    }
}
