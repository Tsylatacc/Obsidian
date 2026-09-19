using Microsoft.AspNetCore.Authorization;
using Obsidian.Application.Features;
using Obsidian.Infrastructure.Abstractions;
using Wolverine;
using Wolverine.Http;

namespace Obsidian.Api.Endpoints
{
    public class SubscriptionEndpoints
    {
        [Authorize]
        [WolverineGet("/api/subscriptions"), RequiresTenant]
        public static async Task<GetSubscriptionResponse> Get(
            IMessageBus bus,
            ICurrentUser currentUser,
            CancellationToken cancellationToken)
        {
            return await bus.InvokeForTenantAsync<GetSubscriptionResponse>(
                currentUser.TenantId.ToString(),
                new GetSubscriptionCommand(),
                cancellationToken);
        }
    }
}
