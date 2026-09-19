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
        [WolverineGet("/api/users"), RequiresTenant]
        public static async Task<GetUserResponse> GetUser(
            IMessageBus bus,
            ICurrentUser currentUser,
            CancellationToken cancellationToken)
        {
            return await bus.InvokeForTenantAsync<GetUserResponse>(
                currentUser.TenantId.ToString(),
                new GetUserCommand(),
                cancellationToken);
        }

        [Authorize]
        [WolverineGet("/api/users/campaigns"), RequiresTenant]
        public static async Task<GetUserCampaignsResponse> GetCampaigns(
            IMessageBus bus,
            ICurrentUser currentUser,
            CancellationToken cancellationToken)
        {
            return await bus.InvokeForTenantAsync<GetUserCampaignsResponse>(
                currentUser.TenantId.ToString(),
                new GetUserCampaignsCommand(),
                cancellationToken);
        }

        [Authorize]
        [WolverineGet("/api/users/usage"), RequiresTenant]
        public static async Task<GetUserUsageResponse> GetUsage(
            IMessageBus bus,
            ICurrentUser currentUser,
            CancellationToken cancellationToken)
        {
            return await bus.InvokeForTenantAsync<GetUserUsageResponse>(
                currentUser.TenantId.ToString(),
                new GetUserUsageCommand(),
                cancellationToken);
        }

        [Authorize]
        [WolverineGet("/api/users/subscriptions"), RequiresTenant]
        public static async Task<GetSubscriptionResponse> GetSubscription(
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
