using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Obsidian.Domain.Entities;
using Obsidian.Domain.Enums;
using Obsidian.Domain.ValueObjects;
using Obsidian.Infrastructure.Abstractions;
using Obsidian.Infrastructure.Persistence;
using Wolverine;

namespace Obsidian.Application.Features;

public class CampaignHandler
{
    public static async Task Handle(
        CampaignCommand command,
        ObsidianDbContext db,
        ICurrentUser currentUser,
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

        User user = await db.Users
            .Where(x => x.Id == currentUser.UserId)
            .Include(x => x.Subscription)
                .ThenInclude(x => x.SubscriptionUsages)
            .SingleOrDefaultAsync(cancellationToken) ??
            throw new KeyNotFoundException(
                $"User {currentUser.UserId} not found.");

        SubscriptionUsage subscriptionUsage =
            user.Subscription.SubscriptionUsages
                .OrderByDescending(x => x.PeriodStartedAt)
                .FirstOrDefault() ?? throw new InvalidOperationException(
                    "Last subscription usage was not found.");

        user.Subscription.EnsureIsValid();
        subscriptionUsage.EnsureIsValid(
            command.Items.Count, 
            command.PhoneNumbers.Count);

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
        
        subscriptionUsage.AddUsage(
            command.Items.Count,
            command.PhoneNumbers.Count);

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
