using Microsoft.EntityFrameworkCore;
using Obsidian.Domain.Enums;
using Obsidian.Infrastructure.Abstractions;
using Obsidian.Infrastructure.Persistence;

namespace Obsidian.Application.Features;

public static class GetSubscriptionHandler
{
    public static async Task<GetSubscriptionResponse> Handle(
        GetSubscriptionCommand command,
        ObsidianDbContext db,
        ICurrentUser currentUser,
        CancellationToken cancellationToken)
    {
        var subscriptionData = await db.Users
            .AsNoTracking()
            .Where(x => x.Id == currentUser.UserId)
            .Select(x => new
            {
                x.Subscription.Id,
                x.Subscription.ExpiresAt,
                x.Subscription.Status,
                x.Subscription.Plan.Type,
                x.Subscription.Plan.Name,
                x.Subscription.Plan.Price,
                x.Subscription.Plan.Description,
                x.Subscription.Plan.CampaignLimit,
                x.Subscription.Plan.MessageLimit,
                x.Subscription.Plan.ChannelLimit,
                x.Subscription.Plan.RecipientPerCampaignLimit
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new KeyNotFoundException(
                $"Subscription for user {currentUser.UserId} was not found.");

        IReadOnlyCollection<GetPaymentResult> payments = await db.Payments
            .AsNoTracking()
            .Where(x => x.SubscriptionId == subscriptionData.Id)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new GetPaymentResult(
                x.Id,
                x.Amount,
                x.ExternalId,
                x.CheckoutUrl,
                x.Status,
                x.CreatedAt,
                x.PaidAt))
            .ToListAsync(cancellationToken);

        GetSubscriptionResult subscription = new(
            subscriptionData.Id,
            subscriptionData.ExpiresAt,
            subscriptionData.Status,
            subscriptionData.Type,
            subscriptionData.Name,
            subscriptionData.Price,
            subscriptionData.Description,
            subscriptionData.CampaignLimit,
            subscriptionData.MessageLimit,
            subscriptionData.ChannelLimit,
            subscriptionData.RecipientPerCampaignLimit,
            payments);

        return new GetSubscriptionResponse(subscription);
    }
}

public sealed record GetSubscriptionCommand();

public sealed record GetSubscriptionResponse(
    GetSubscriptionResult Subscription
);

public sealed record GetSubscriptionResult(
    Guid Id,
    DateTimeOffset ExpiresAt,
    SubscriptionStatus Status,
    SubscriptionPlanType Type,
    string Name,
    decimal Price,
    string? Description,
    int CampaignLimit,
    int MessageLimit,
    int ChannelLimit,
    int RecipientPerCampaignLimit,
    IReadOnlyCollection<GetPaymentResult> Payments
);

public sealed record GetPaymentResult(
    Guid Id,
    decimal Amount,
    string? ExternalId,
    string? CheckoutUrl,
    PaymentStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? PaidAt
);