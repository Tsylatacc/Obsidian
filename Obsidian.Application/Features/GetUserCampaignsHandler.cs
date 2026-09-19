using Microsoft.EntityFrameworkCore;
using Obsidian.Domain.Enums;
using Obsidian.Infrastructure.Abstractions;
using Obsidian.Infrastructure.Persistence;

namespace Obsidian.Application.Features;

public static class GetUserCampaignsHandler
{
    public static async Task<GetUserCampaignsResponse> Handle(
        GetUserCampaignsCommand command,
        ObsidianDbContext db,
        ICurrentUser currentUser,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<GetUserCampaignResult> campaigns = await db.Campaigns
            .AsNoTracking()
            .Where(x => x.TenantId == currentUser.TenantId.ToString())
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new GetUserCampaignResult(
                x.Id,
                x.Name,
                x.Status,
                x.ChannelId,
                x.Recipients.Count,
                x.Contents.Count,
                x.CreatedAt,
                x.FinishedAt))
            .ToListAsync(cancellationToken);

        return new GetUserCampaignsResponse(campaigns);
    }
}

public sealed record GetUserCampaignsCommand();

public sealed record GetUserCampaignsResponse(
    IReadOnlyCollection<GetUserCampaignResult> Campaigns
);

public sealed record GetUserCampaignResult(
    Guid Id,
    string Name,
    CampaignStatus Status,
    Guid ChannelId,
    int RecipientCount,
    int ContentCount,
    DateTimeOffset CreatedAt,
    DateTimeOffset? FinishedAt
);
