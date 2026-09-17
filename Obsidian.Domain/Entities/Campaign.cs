using JasperFx.MultiTenancy;
using Obsidian.Domain.Enums;
using Obsidian.Domain.ValueObjects;

namespace Obsidian.Domain.Entities;

public class Campaign : ITenanted
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;

    public Guid ChannelId { get; private set; }
    public Channel Channel { get; private set; } = default!;

    public CampaignStatus Status { get; private set; }

    private readonly List<CampaignContent> _contents = [];

    public IReadOnlyCollection<CampaignContent> Contents =>
        _contents.AsReadOnly();

    private readonly List<Recipient> _recipients = [];

    public IReadOnlyCollection<Recipient> Recipients =>
        _recipients.AsReadOnly();

    public string? TenantId { get; set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? FinishedAt { get; private set; }

    private Campaign()
    {
    }

    private Campaign(Guid channelId)
    {
        Id = Guid.NewGuid();
        Name = $"Campaign {Guid.NewGuid().ToString()[..8].ToUpper()}";
        ChannelId = channelId;
        Status = CampaignStatus.Pending;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public static Campaign Create(Guid channelId)
    {
        if (channelId == Guid.Empty)
            throw new ArgumentException(
                "ChannelId is required.",
                nameof(channelId));

        return new Campaign(channelId);
    }

    public void SetContents(
        IReadOnlyCollection<CampaignContent> contents)
    {
        if (contents.Count == 0)
            throw new ArgumentException(
                "At least one content is required.",
                nameof(contents));

        if (contents
            .Select(x => x.Position)
            .Distinct()
            .Count() != contents.Count)
        {
            throw new ArgumentException(
                "Content positions must be unique.",
                nameof(contents));
        }

        _contents.Clear();

        _contents.AddRange(
            contents.OrderBy(x => x.Position));
    }

    public void SetRecipients(
        IReadOnlyCollection<Recipient> recipients)
    {
        if (recipients.Count == 0)
            throw new ArgumentException(
                "At least one recipient is required.",
                nameof(recipients));

        _recipients.AddRange(recipients.Distinct());
    }
}