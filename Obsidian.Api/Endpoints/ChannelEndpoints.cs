using Microsoft.AspNetCore.Authorization;
using Obsidian.Application.Features;
using Obsidian.Infrastructure.Abstractions;
using Wolverine;
using Wolverine.Http;

namespace Obsidian.Api.Endpoints
{
    public class ChannelEndpoints
    {

        [Authorize]
        [WolverinePost("/api/channels"), RequiresTenant]
        public static async Task<CreateChannelResponse> Create(
            CreateChannelCommand command,
            IMessageBus bus,
            ICurrentUser currentUser,
            CancellationToken cancellationToken)
        {
            return await bus.InvokeForTenantAsync<CreateChannelResponse>(
                currentUser.TenantId.ToString(),
                command, 
                cancellationToken);
        }

        [Authorize]
        [WolverinePut("/api/channels/connect"), RequiresTenant]
        public static async Task<ConnectChannelResponse> Connect(
            ConnectChannelCommand command,
            IMessageBus bus,
            ICurrentUser currentUser,
            CancellationToken cancellationToken)
        {
            return await bus.InvokeForTenantAsync<ConnectChannelResponse>(
                currentUser.TenantId.ToString(), 
                command, 
                cancellationToken);
        }

        [Authorize]
        [WolverineDelete("/api/channels/delete"), RequiresTenant]
        public static async Task Delete(
            DeleteChannelCommand command,
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
