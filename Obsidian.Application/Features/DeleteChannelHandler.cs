using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Obsidian.Domain.Entities;
using Obsidian.Infrastructure.ExceptionHandling;
using Obsidian.Infrastructure.Options;
using Obsidian.Infrastructure.Persistence;
using System.Net.Http.Json;

namespace Obsidian.Application.Features
{
    public class DeleteChannelHandler
    {
        public static async Task Handle(
             DeleteChannelCommand command,
             ObsidianDbContext db,
             HttpClient httpClient,
             IOptions<EvolutionApiOptions> options,
             CancellationToken cancellationToken)
        {
            Channel channel = await db.Channels
                .SingleOrDefaultAsync(x => x.Id == command.ChannelId, cancellationToken)
                ?? throw new ConflictException($"Channel {command.ChannelId} not found");

            HttpRequestMessage request = new(
                HttpMethod.Get,
                $"{options.Value.ApiUrl}/instance/connect/{channel.Name}");

            request.Headers.Add("apikey", options.Value.ApiKey);

            HttpResponseMessage response = await httpClient.SendAsync(
                request,
                cancellationToken);
            response.EnsureSuccessStatusCode();

            DeleteInstanceResponse result = await response.Content
                .ReadFromJsonAsync<DeleteInstanceResponse>(
                cancellationToken) ??
                    throw new InvalidOperationException("Evolution Api returned an empty response.");

            if (!result.Success.Equals("success", StringComparison.CurrentCultureIgnoreCase))
                throw new InvalidOperationException($"Channel {channel.Id} wasn't successfully deleted.");

            await db.Channels
                .Where(x => x.Id == channel.Id)
                .ExecuteDeleteAsync(cancellationToken);
        }

    }
    public sealed record DeleteChannelCommand(
Guid ChannelId
);
    public sealed record DeleteInstanceResponse(
        string Success);
}
