using Microsoft.EntityFrameworkCore;
using Obsidian.Domain.Enums;
using Obsidian.Infrastructure.Persistence;

namespace Obsidian.Application.Features
{
    public class GetSubscriptionPlansHandler
    {
        public static async Task<GetSubscriptionPlansResponse> Handle(
             GetSubscriptionPlansCommand command,
             ObsidianDbContext db,
             CancellationToken cancellationToken)
        {
            IReadOnlyCollection<GetSubscriptionPlansResult> result = await db.SubscriptionPlans
                .AsNoTracking()
                .IgnoreQueryFilters()
                .Where(x => x.IsActive)
                .Select(x => new GetSubscriptionPlansResult(
                    x.Id,
                    x.Name,
                    x.Type,
                    x.Price,
                    x.Duration,
                    x.Description,
                    x.CampaignLimit,
                    x.MessageLimit,
                    x.ChannelLimit,
                    x.RecipientPerCampaignLimit))
                .ToListAsync(cancellationToken);

            return new GetSubscriptionPlansResponse(result);
        }
    }
    public sealed record GetSubscriptionPlansCommand();
    public sealed record GetSubscriptionPlansResponse(
        IReadOnlyCollection<GetSubscriptionPlansResult> SubscriptionPlans
    );
    public sealed record GetSubscriptionPlansResult(
        Guid Id,
        string Name,
        SubscriptionPlanType Type,
        decimal Price,
        TimeSpan Duration,
        string? Description,
        int CampaignLimit,
        int MessageLimit,
        int ChannelLimit,
        int RecipientPerCampaignLimit
    );
}
