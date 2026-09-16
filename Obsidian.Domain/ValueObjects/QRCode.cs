namespace Obsidian.Domain.ValueObjects
{
    public sealed record QRCode
    {
        public string? PairingCode { get; private set; }
        public string Code { get; private set; } = default!;

        private QRCode(
        string? pairingCode,
        string code)
        {
            PairingCode = pairingCode;
            Code = code;
        }

        public static QRCode Create(
            string? pairingCode,
            string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException(
                    "QR Code is required.",
                    nameof(code));

            return new QRCode(
                pairingCode,
                code);
        }
    }
}
