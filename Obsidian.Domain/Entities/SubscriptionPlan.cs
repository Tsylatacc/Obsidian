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

        public int MessageLimit { get; private set; }
        public int ChannelLimit { get; private set; }

        public bool IsActive { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset UpdatedAt { get; private set; }

        private SubscriptionPlan() { } // EF Core

        private SubscriptionPlan(
            string name,
            decimal price,
            int messageLimit,
            int channelLimit,
            SubscriptionPlanType type,
            TimeSpan duration)
        {
            Id = Guid.NewGuid();
            Name = name;
            Price = price;
            MessageLimit = messageLimit;
            ChannelLimit = channelLimit;
            IsActive = true;
            Type = type;
            Duration = duration;
        }

        public static SubscriptionPlan Create(
            string name,
            decimal price,
            int messageLimit,
            int channelLimit,
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

            if (messageLimit <= 0)
                throw new ArgumentException(
                    "Argument 'messageLimit ' cannot be equal or less than zero.",
                    nameof(messageLimit));

            if (channelLimit <= 0)
                throw new ArgumentException(
                    "Argument 'channelLimit' cannot be equal or less than zero.",
                    nameof(channelLimit));

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
                messageLimit,
                channelLimit,
                type,
                duration);
        }
    }
}