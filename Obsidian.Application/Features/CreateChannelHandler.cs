using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Obsidian.Domain.Entities;
using Obsidian.Domain.Enums;
using Obsidian.Domain.ValueObjects;
using Obsidian.Infrastructure.ExceptionHandling;
using Obsidian.Infrastructure.Options;
using Obsidian.Infrastructure.Persistence;
using Obsidian.Infrastructure.Security;
using System.Net.Http.Json;

namespace Obsidian.Application.Features
{
    public class CreateChannelHandler
    {
        private static readonly string[] _events =
                    [
                        "CONNECTION_UPDATE",
                        "LOGOUT_INSTANCE"
                    ];

        public static async Task<CreateChannelResponse> Handle(
             CreateChannelCommand command,
             ObsidianDbContext db,
             HttpClient httpClient,
             IOptions<EvolutionApiOptions> options,
             CancellationToken cancellationToken)
        {
            var phone = PhoneNumber.Create(command.PhoneNumber);

            if (await db.Channels.AnyAsync(x => x.Name == command.Name ||
                x.PhoneNumber == phone, cancellationToken))
                throw new ConflictException($"These channel parameters are already being used.");

            Channel channel = Channel.Create(command.Name, SecurityService.GenerateToken(32), phone);

            HttpRequestMessage request = new(
                HttpMethod.Post,
                $"{options.Value.ApiUrl}/instance/create");

            request.Headers.Add("apikey", options.Value.ApiKey);

            request.Content = JsonContent.Create(new
            {
                instanceName = channel.Name,
                qrcode = true,
                integration = channel.Integration,
                token = channel.Token,
                number = channel.PhoneNumber,
                webhook = new
                {
                    enabled = true,
                    events = _events
                }
            });

            HttpResponseMessage response = await httpClient.SendAsync(
                request,
                cancellationToken);
            response.EnsureSuccessStatusCode();

            CreateInstanceResponse result = await response.Content
                .ReadFromJsonAsync<CreateInstanceResponse>(
                cancellationToken) ??
                    throw new InvalidOperationException("Evolution Api returned an empty response.");

            if (!result.Instance.Status.Equals(ChannelStatus.Connecting.ToString(), StringComparison.CurrentCultureIgnoreCase))
                throw new InvalidOperationException($"Incompatible channel {channel.Id} status.");

            channel.SetQRCode(result.Qrcode.PairingCode, result.Qrcode.Code);

            return new CreateChannelResponse(
                result.Qrcode.Base64);
        }
    }
    public sealed record CreateChannelCommand(
        string Name,
        string PhoneNumber
    );
    public sealed record CreateChannelResponse(
        string QRCodeBase64
    );
    public sealed record CreateInstanceResponse(
        InstanceResponse Instance,
        QRCodeResponse Qrcode);
    public sealed record InstanceResponse(
        string Status
    );
    public sealed record QRCodeResponse(
        string? PairingCode,
        string Code,
        string Base64
    );
}
