using Microsoft.EntityFrameworkCore;
using Obsidian.Domain.Entities;
using Obsidian.Domain.Enums;

namespace Obsidian.Infrastructure.Persistence;

public static class SubscriptionPlanSeeder
{
    public static async Task SeedAsync(
        ObsidianDbContext db,
        CancellationToken cancellationToken = default)
    {
        if (await db.SubscriptionPlans.AnyAsync(
                x => x.Type == SubscriptionPlanType.Free,
                cancellationToken))
            return;

        SubscriptionPlan freePlan = SubscriptionPlan.Create(
            "Free",
            0,
            5,
            2000,
            1,
            200,
            SubscriptionPlanType.Free,
            TimeSpan.FromDays(30));

        await db.SubscriptionPlans.AddAsync(freePlan, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }
}
