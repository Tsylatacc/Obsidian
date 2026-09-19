using Microsoft.EntityFrameworkCore;
using Obsidian.Domain.Entities;
using Obsidian.Infrastructure.Abstractions;
using Obsidian.Infrastructure.Persistence;

namespace Obsidian.Application.Features
{
    public class GetUserUsageHandler
    {
        public static async Task<GetUserUsageResponse> Handle(
             GetUserUsageCommand command,
             ObsidianDbContext db,
             ICurrentUser currentUser,
             CancellationToken cancellationToken)
        {
            SubscriptionUsage subscriptionUsage = await db.Users
            .Where(x => x.Id == currentUser.UserId)
            .SelectMany(x => x.Subscription.SubscriptionUsages)
            .OrderByDescending(x => x.PeriodStartedAt)
            .FirstOrDefaultAsync(cancellationToken)
                ?? throw new KeyNotFoundException(
                    $"Last subscription usage for user {currentUser.UserId} was not found.");

            return new GetUserUsageResponse(
                subscriptionUsage.CampaignsUsed,
                subscriptionUsage.CampaignsRemaining,
                subscriptionUsage.MessagesUsed,
                subscriptionUsage.MessagesRemaining, 
                subscriptionUsage.ChannelsUsed,
                subscriptionUsage.ChannelsRemaining,
                subscriptionUsage.PeriodStartedAt,
                subscriptionUsage.PeriodExpiresAt);
        }
    }
    public sealed record GetUserUsageCommand();
    public sealed record GetUserUsageResponse(
        int CampaignsUsed,
        int CampaignsRemaining,
        int MessagesUsed,
        int MessagesRemaining,
        int ChannelsUsed,
        int ChannelsRemaining,
        DateTimeOffset PeriodStartedAt,
        DateTimeOffset PeriodExpiresAt
    );
}

