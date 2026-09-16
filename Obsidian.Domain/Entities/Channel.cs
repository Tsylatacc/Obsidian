using JasperFx.MultiTenancy;
using Obsidian.Domain.Enums;
using Obsidian.Domain.ValueObjects;

namespace Obsidian.Domain.Entities
{
    public class Channel : ITenanted
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = default!;
        public string Token { get; private set; } = default!;
        public PhoneNumber PhoneNumber { get; private set; } = default!;
        public ChannelStatus Status { get; private set; }
        public QRCode? QRCode { get; private set; }
        public string Integration { get; private set; } = default!;

        public string? TenantId { get; set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset UpdatedAt { get; private set; }

        private Channel() { } // EF Core

        private Channel(
            string name,
            string token,
            PhoneNumber phoneNumber)
        {
            Id = Guid.NewGuid();
            Name = name;
            Token = token;
            PhoneNumber = phoneNumber;
            Status = ChannelStatus.Connecting;
            Integration = "WHATSAPP-BAILEYS";

            CreatedAt = DateTimeOffset.UtcNow;
            UpdatedAt = CreatedAt;
        }

        public static Channel Create(
            string name,
            string token,
            PhoneNumber phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(
                    "Argument 'name' is required.",
                    nameof(name));
            
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException(
                    "Argument 'token' is required.",
                    nameof(token));

            return new Channel(
                name,
                token,
                phoneNumber);
        }

        public void SetQRCode(string? pairingCode, string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException(
                    "Argument 'code' is required.",
                    nameof(code));

            QRCode = QRCode.Create(pairingCode, code);
        }
    }
}
