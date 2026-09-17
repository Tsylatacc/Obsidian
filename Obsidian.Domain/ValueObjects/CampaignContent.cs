using Obsidian.Domain.Enums;

namespace Obsidian.Domain.ValueObjects;

public sealed record CampaignContent
{
    public int Position { get; }

    public CampaignContentType Type { get; }

    public string? Text { get; }

    public MediaType? MediaType { get; }
    public string? MimeType { get; }
    public string? Caption { get; }
    public string? Url { get; }
    public string? FileName { get; }

    private CampaignContent(
        int position,
        CampaignContentType type,
        string? text,
        MediaType? mediaType,
        string? mimeType,
        string? caption,
        string? url,
        string? fileName)
    {
        Position = position;
        Type = type;
        Text = text;
        MediaType = mediaType;
        MimeType = mimeType;
        Caption = caption;
        Url = url;
        FileName = fileName;
    }

    public static CampaignContent CreateText(
        int position,
        string text)
    {
        if (position < 0)
            throw new ArgumentException(
                "Position cannot be negative.",
                nameof(position));

        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException(
                "Text is required.",
                nameof(text));

        return new CampaignContent(
            position,
            CampaignContentType.Text,
            text,
            null,
            null,
            null,
            null,
            null);
    }

    public static CampaignContent CreateMedia(
        int position,
        MediaType type,
        string mimeType,
        string? caption,
        string url,
        string fileName)
    {
        if (position < 0)
            throw new ArgumentException(
                "Position cannot be negative.",
                nameof(position));

        if (string.IsNullOrWhiteSpace(mimeType))
            throw new ArgumentException(
                "MimeType is required.",
                nameof(mimeType));

        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException(
                "Url is required.",
                nameof(url));

        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException(
                "FileName is required.",
                nameof(fileName));

        return new CampaignContent(
            position,
            CampaignContentType.Media,
            null,
            type,
            mimeType,
            caption,
            url,
            fileName);
    }
}