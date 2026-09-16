using Obsidian.Domain.Enums;

namespace Obsidian.Domain.ValueObjects
{
    public sealed record CampaignMedia
    {
        public MediaType Type { get; }
        public string MimeType { get; }
        public string? Caption { get; }
        public string Url { get; }
        public string FileName { get; }

        private CampaignMedia(
            MediaType type,
            string mimeType,
            string? caption,
            string url,
            string fileName)
        {
            Type = type;
            MimeType = mimeType;
            Caption = caption;
            Url = url;
            FileName = fileName;
        }

        public static CampaignMedia Create(
            MediaType type,
            string mimeType,
            string? caption,
            string url,
            string fileName)
        {
            if (string.IsNullOrWhiteSpace(mimeType))
                throw new ArgumentException(
                    "MIME type is required.",
                    nameof(mimeType));

            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException(
                    "URL is required.",
                    nameof(url));

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException(
                    "File name is required.",
                    nameof(fileName));

            return new CampaignMedia(
                type,
                mimeType,
                caption,
                url,
                fileName);
        }
    }
}
