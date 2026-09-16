using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Obsidian.Domain.Entities;
using Obsidian.Infrastructure.ExceptionHandling;
using Obsidian.Infrastructure.Options;
using Obsidian.Infrastructure.Persistence;
using System.Net.Http.Json;

namespace Obsidian.Application.Features
{
    public class ConnectChannelHandler
    {
        public static async Task<ConnectChannelResponse> Handle(
             ConnectChannelCommand command,
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

            ConnectInstanceResponse result = await response.Content
                .ReadFromJsonAsync<ConnectInstanceResponse>(
                cancellationToken) ??
                    throw new InvalidOperationException("Evolution Api returned an empty response.");

            channel.SetQRCode(result.PairingCode, result.Code);

            return new ConnectChannelResponse(
                result.Base64);
        }
    }
    public sealed record ConnectChannelCommand(
Guid ChannelId
);
    public sealed record ConnectChannelResponse(
        string QRCodeBase64
    );
    public sealed record ConnectInstanceResponse(
        string? PairingCode,
        string Code,
        string Base64);
}
