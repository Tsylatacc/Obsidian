using Obsidian.Domain.Enums;

namespace Obsidian.Domain.Entities
{
    public class SubscriptionPlan
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = default!;
        public SubscriptionPlanType Type { get; private set; }

        public decimal Price { get; private set; }
        public TimeSpan Duration { get; private set; }
        public string? Description { get; private set; } = default!;

        public int CampaignLimit { get; private set; }
        public int MessageLimit { get; private set; }
        public int ChannelLimit { get; private set; }

        public int RecipientPerCampaignLimit { get; private set; }

        public bool IsActive { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset UpdatedAt { get; private set; }

        private SubscriptionPlan() { } // EF Core

        private SubscriptionPlan(
            string name,
            decimal price,
            int campaignLimit,
            int messageLimit,
            int channelLimit,
            int recipientPerCampaignLimit,
            SubscriptionPlanType type,
            TimeSpan duration)
        {
            Id = Guid.NewGuid();
            Name = name;
            Price = price;
            CampaignLimit = campaignLimit;
            MessageLimit = messageLimit;
            ChannelLimit = channelLimit;
            RecipientPerCampaignLimit = recipientPerCampaignLimit;
            IsActive = true;
            Type = type;
            Duration = duration;
        }

        public static SubscriptionPlan Create(
            string name,
            decimal price,
            int campaignLimit,
            int messageLimit,
            int channelLimit,
            int recipientPerCampaignLimit,
            SubscriptionPlanType type,
            TimeSpan duration)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(
                    "Argument 'name' cannot be empty.",
                    nameof(name));

            if (price < 0)
                throw new ArgumentException(
                    "Argument 'price' cannot be negative.",
                    nameof(price));

            if (campaignLimit <= 0)
                throw new ArgumentException(
                    "Argument 'campaignLimit' cannot be equal or less than zero.",
                    nameof(campaignLimit));

            if (messageLimit <= 0)
                throw new ArgumentException(
                    "Argument 'messageLimit ' cannot be equal or less than zero.",
                    nameof(messageLimit));

            if (channelLimit <= 0)
                throw new ArgumentException(
                    "Argument 'channelLimit' cannot be equal or less than zero.",
                    nameof(channelLimit));

            if (recipientPerCampaignLimit <= 0)
                throw new ArgumentException(
                    "Argument 'recipientPerCampaignLimit' cannot be equal or less than zero.",
                    nameof(recipientPerCampaignLimit));

            if (!Enum.IsDefined(type))
                throw new ArgumentException(
                    "Argument 'type' is invalid.",
                    nameof(type));

            if (duration <= TimeSpan.Zero)
                throw new ArgumentException(
                    "Argument 'duration' must be greater than zero.",
                    nameof(duration));

            return new SubscriptionPlan(
                name,
                price,
                campaignLimit,
                messageLimit,
                channelLimit,
                recipientPerCampaignLimit,
                type,
                duration);
        }
    }
}