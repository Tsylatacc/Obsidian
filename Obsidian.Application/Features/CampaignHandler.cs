using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Obsidian.Domain.Entities;
using Obsidian.Domain.Enums;
using Obsidian.Domain.ValueObjects;
using Obsidian.Infrastructure.Persistence;
using Wolverine;

namespace Obsidian.Application.Features;

public class CampaignHandler
{
    public static async Task Handle(
        CampaignCommand command,
        ObsidianDbContext db,
        IMessageBus bus,
        CancellationToken cancellationToken)
    {
        if (!await db.Channels.AnyAsync(
                x => x.Id == command.ChannelId,
                cancellationToken))
        {
            throw new KeyNotFoundException(
                $"Channel {command.ChannelId} not found.");
        }

        if (command.PhoneNumbers.Count == 0)
            throw new ArgumentException(
                "At least one phone number is required.",
                nameof(command.PhoneNumbers));

        if (command.PhoneNumbers.Count > 10000)
            throw new ArgumentException(
                "Maximum of 10,000 phone numbers.",
                nameof(command.PhoneNumbers));

        if (command.Items.Count == 0)
            throw new ArgumentException(
                "At least one item is required.",
                nameof(command.Items));

        List<CampaignContent> contents = [];

        FileExtensionContentTypeProvider provider = new();

        for (int position = 0; position < command.Items.Count; position++)
        {
            CampaignItem item = command.Items[position];

            switch (item)
            {
                case TextItem text:
                    {
                        contents.Add(
                            CampaignContent.CreateText(
                                position,
                                text.Content));

                        break;
                    }

                case MediaItem media:
                    {
                        if (!provider.TryGetContentType(
                                media.FileName,
                                out string? mimeType))
                        {
                            throw new ArgumentException(
                                $"Invalid file type: {media.FileName}");
                        }

                        contents.Add(
                            CampaignContent.CreateMedia(
                                position,
                                media.Type,
                                mimeType,
                                media.Caption,
                                media.Url,
                                media.FileName));

                        break;
                    }

                default:
                    throw new ArgumentException(
                        $"Unsupported item type: {item.GetType().Name}");
            }
        }

        Campaign campaign =
            Campaign.Create(command.ChannelId);

        campaign.SetContents(contents);

        List<Recipient> recipients = [];

        foreach (string phoneNumber in command.PhoneNumbers.Distinct())
        {
            recipients.Add(
                Recipient.Create(
                    campaign.Id,
                    phoneNumber));
        }

        campaign.SetRecipients(recipients);

        await db.Campaigns.AddAsync(
            campaign,
            cancellationToken);

        await bus.PublishAsync(
            new CampaignRequested(
                campaign.Id,
                campaign.ChannelId));
    }
}

public sealed record CampaignCommand(
    IReadOnlyCollection<string> PhoneNumbers,
    List<CampaignItem> Items,
    Guid ChannelId
);

public abstract record CampaignItem;

public sealed record TextItem(
    string Content
) : CampaignItem;

public sealed record MediaItem(
    MediaType Type,
    string Url,
    string FileName,
    string? Caption = null
) : CampaignItem;

public sealed record CampaignRequested(
    Guid CampaignId,
    Guid ChannelId
);