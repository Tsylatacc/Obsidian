using JasperFx.MultiTenancy;
using Obsidian.Domain.Enums;
using Obsidian.Domain.ValueObjects;

namespace Obsidian.Domain.Entities
{
    public class Recipient : ITenanted
    {
        public Guid Id { get; private set; }
        public Guid CampaignId { get; private set; }
        public PhoneNumber PhoneNumber { get; private set; } = default!;
        public DeliveryStatus Status { get; private set; }

        public string? TenantId { get; set; }
        public DateTimeOffset? DeliveredAt { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }

        private Recipient() { } // EF Core

        private Recipient(
            Guid campaignId,
            PhoneNumber phoneNumber)
        {
            Id = Guid.NewGuid();
            CampaignId = campaignId;
            PhoneNumber = phoneNumber;
            Status = DeliveryStatus.Pending;

            CreatedAt = DateTimeOffset.UtcNow;
        }

        public static Recipient Create(
            Guid campaignId,
            string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException(
                    "Argument 'phoneNumber' cannot be empty.",
                    nameof(phoneNumber));

            if (campaignId == Guid.Empty)
                throw new ArgumentException(
                    "Argument 'campaignId' cannot be empty.",
                    nameof(campaignId));

            var phone = PhoneNumber.Create(phoneNumber);

            return new Recipient(
                campaignId,
                phone);
        }
    }
}
