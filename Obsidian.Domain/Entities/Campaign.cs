using Obsidian.Domain.Enums;
using Obsidian.Domain.ValueObjects;
using JasperFx.MultiTenancy;


namespace Obsidian.Domain.Entities
{
    public class Campaign : ITenanted
    {
        public Guid Id { get; private set; }
        public string Message { get; private set; } = default!;

        public Guid ChannelId { get; private set; }
        public Channel Channel { get; private set; } = default!;

        public CampaignMedia? Media { get; private set; }
        public CampaignStatus Status { get; private set; }

        private readonly List<Recipient> _recipients = [];
        public IReadOnlyCollection<Recipient> Recipients => _recipients.AsReadOnly();

        public string? TenantId { get; set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset? FinishedAt { get; private set; }

        private Campaign() { } // EF Core

        private Campaign(
            string message,
            Guid channelId,
            CampaignMedia? media)
        {
            Id = Guid.NewGuid();
            Message = message;
            ChannelId = channelId;
            Media = media;
            Status = CampaignStatus.Pending;

            CreatedAt = DateTimeOffset.UtcNow;
        }

        public static Campaign Create(
        string message,
        Guid channelId,
        CampaignMedia? media = null)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException(
                    "Message is required.",
                    nameof(message));

            if (channelId == Guid.Empty)
                throw new ArgumentException(
                    "ChannelId is required.",
                    nameof(message));

            return new Campaign(
                message,
                channelId,
                media);
        }

        public void SetRecipients(IReadOnlyCollection<Recipient> deliveries)
        {
            if (deliveries.Count == 0)
                throw new ArgumentException("At least one recipient is required.");

            _recipients.AddRange(deliveries.Distinct());
        }
    }
}
