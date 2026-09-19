using Microsoft.AspNetCore.Authorization;
using Obsidian.Application.Features;
using Wolverine;
using Wolverine.Http;

namespace Obsidian.Api.Endpoints
{
    public class SubscriptionPlanEndpoints
    {
        [AllowAnonymous]
        [WolverineGet("/api/plans"), NotTenanted]
        public static async Task<GetSubscriptionPlansResponse> Get(
            IMessageBus bus,
            CancellationToken cancellationToken)
        {
            return await bus.InvokeAsync<GetSubscriptionPlansResponse>(
                new GetSubscriptionPlansCommand(),
                cancellationToken);
        }
    }
}
