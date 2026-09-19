using Microsoft.AspNetCore.Authorization;
using Obsidian.Application.Features;
using Obsidian.Infrastructure.Abstractions;
using Wolverine;
using Wolverine.Http;

namespace Obsidian.Api.Endpoints
{
    public class UserEndpoints

    {
        [Authorize]
        [WolverineGet("/api/users/usage"), RequiresTenant]
        public static async Task<GetSubscriptionPlansResponse> GetUsage(
            IMessageBus bus,
            ICurrentUser currentUser,
            CancellationToken cancellationToken)
        {
            return await bus.InvokeForTenantAsync<GetSubscriptionPlansResponse>(
                currentUser.TenantId.ToString(),
                cancellationToken);
        }

        [Authorize]
        [WolverineGet("/api/users/subscriptions"), RequiresTenant]
        public static async Task<GetSubscriptionPlansResponse> GetSubscription(
            IMessageBus bus,
            ICurrentUser currentUser,
            CancellationToken cancellationToken)
        {
            return await bus.InvokeForTenantAsync<GetSubscriptionPlansResponse>(
                currentUser.TenantId.ToString(),
                cancellationToken);
        }
    }
}
