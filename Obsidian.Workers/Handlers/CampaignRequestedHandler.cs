using Microsoft.EntityFrameworkCore;
using Obsidian.Application.Features;
using Obsidian.Domain.Entities;
using Obsidian.Domain.Enums;
using Obsidian.Domain.ValueObjects;
using Obsidian.Infrastructure.Persistence;

namespace Obsidian.Workers.Handlers;

public sealed class CampaignRequestedHandler
{
    public static async Task Handle(
        CampaignRequested message,
        ObsidianDbContext db,
        CancellationToken cancellationToken)
    {
        Campaign campaign = await db.Campaigns
            .Include(x => x.Contents)
            .Include(x => x.Recipients)
            .SingleOrDefaultAsync(
                x => x.Id == message.CampaignId,
                cancellationToken)
            ?? throw new KeyNotFoundException(
                $"Campaign {message.CampaignId} not found");

        if (campaign.Status != CampaignStatus.Pending)
            throw new InvalidOperationException(
                $"Campaign {message.CampaignId} is not a valid state.");

        foreach (Recipient recipient in campaign.Recipients)
        {
            foreach (CampaignContent content in campaign.Contents
                .OrderBy(x => x.Position))
            {
                switch (content.Type)
                {
                    case CampaignContentType.Text:
                        break;

                    case CampaignContentType.Media:
                        break;

                    default:
                        throw new InvalidOperationException(
                            $"Unsupported campaign content type: {content.Type}");
                }
            }
        }
    }
}