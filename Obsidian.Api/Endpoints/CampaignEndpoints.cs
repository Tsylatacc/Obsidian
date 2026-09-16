using Microsoft.AspNetCore.Authorization;
using Obsidian.Application.Features;
using Obsidian.Infrastructure.Abstractions;
using Wolverine;
using Wolverine.Http;

namespace Obsidian.Api.Endpoints
{
    public class CampaignEndpoints
    {
        [Authorize]
        [WolverinePost("/api/campaign/start"), RequiresTenant]
        public static async Task StartCampaign(
            CampaignCommand command,
            IMessageBus bus,
            ICurrentUser currentUser,
            CancellationToken cancellationToken)
        {
            await bus.InvokeForTenantAsync(
                currentUser.TenantId.ToString(), 
                command, 
                cancellationToken);
        }
    }


}
