using Microsoft.AspNetCore.Authorization;
using Obsidian.Infrastructure.Abstractions;
using Wolverine;
using Wolverine.Http;

namespace Obsidian.Api.Endpoints
{
    public class SubscriptionEndpoints
    {
        [Authorize]
        [WolverinePost("/api/subscriptions"), RequiresTenant]
        public static async Task<UpdateSubscriptionResponse> Update(
            UpdateSubscriptionCommand command,
            IMessageBus bus,
            ICurrentUser currentUser,
            CancellationToken cancellationToken)
        {
            return await bus.InvokeForTenantAsync<UpdateSubscriptionResponse>(
                currentUser.TenantId.ToString(), 
                command, 
                cancellationToken);
        }
    }

    public sealed record UpdateSubscriptionCommand(
        string Name,
        string PhoneNumber
    );
    public sealed record UpdateSubscriptionResponse(
        string QRCodeBase64
    );
}
