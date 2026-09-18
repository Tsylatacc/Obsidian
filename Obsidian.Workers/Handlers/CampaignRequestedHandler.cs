using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Obsidian.Application.Features;
using Obsidian.Domain.Entities;
using Obsidian.Domain.Enums;
using Obsidian.Domain.ValueObjects;
using Obsidian.Infrastructure.Options;
using Obsidian.Infrastructure.Persistence;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Obsidian.Workers.Handlers;

public sealed class CampaignRequestedHandler
{
    public static async Task Handle(
        CampaignRequested message,
        ObsidianDbContext db,
        HttpClient httpClient,
        IOptions<EvolutionApiOptions> options,
        CancellationToken cancellationToken)
    {
        Channel channel = await db.Channels
            .SingleOrDefaultAsync(
                x => x.Id == message.ChannelId,
                cancellationToken)
            ?? throw new KeyNotFoundException(
                $"Channel {message.ChannelId} not found");

        Campaign campaign = await db.Campaigns
            .Include(x => x.Contents)
            .Include(x => x.Recipients)
            .SingleOrDefaultAsync(
                x => x.Id == message.CampaignId,
                cancellationToken)
            ?? throw new KeyNotFoundException(
                $"Campaign {message.CampaignId} not found");

        if (campaign.Status != CampaignStatus.Pending)
            throw new InvalidOperationException(
                $"Campaign {message.CampaignId} is not a valid state.");

        if (channel.Status != ChannelStatus.Open)
        {
            foreach (Recipient recipient in campaign.Recipients)
                recipient.DeliveryFailed();

            await db.SaveChangesAsync(cancellationToken);

            return;
        }

        CampaignContent[] contents = campaign.Contents
            .OrderBy(x => x.Position)
            .ToArray();

        foreach (Recipient recipient in campaign.Recipients)
        {
            bool successful = true;

            foreach (CampaignContent content in contents)
            {
                int delay = Random.Shared.Next(2000, 6001);
                try
                {
                    using HttpRequestMessage request = await CreateRequest(
                        content,
                        recipient,
                        channel,
                        options.Value,
                        httpClient,
                        delay,
                        cancellationToken);

                    using HttpResponseMessage response =
                        await httpClient.SendAsync(
                            request,
                            cancellationToken);

                    response.EnsureSuccessStatusCode();
                }
                catch
                {
                    successful = false;
                    break;
                }
            }

            if (successful)
                recipient.SuccessfulDelivery();
            else
                recipient.DeliveryFailed();
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task<HttpRequestMessage> CreateRequest(
        CampaignContent content,
        Recipient recipient,
        Channel channel,
        EvolutionApiOptions options,
        HttpClient httpClient,
        int delay,
        CancellationToken cancellationToken)
    {
        
        HttpRequestMessage request = content.Type switch
        {
            CampaignContentType.Text => CreateTextRequest(
                content,
                recipient,
                channel,
                options,
                delay),

            CampaignContentType.Media => await CreateMediaRequest(
                content,
                recipient,
                channel,
                options,
                httpClient,
                delay,
                cancellationToken),

            _ => throw new InvalidOperationException(
                $"Unsupported campaign content type: {content.Type}")
        };

        request.Headers.Add("apikey", options.ApiKey);

        return request;
    }

    private static HttpRequestMessage CreateTextRequest(
        CampaignContent content,
        Recipient recipient,
        Channel channel,
        EvolutionApiOptions options,
        int delay)
    {
        return new HttpRequestMessage(
            HttpMethod.Post,
            $"{options.ApiUrl}/message/{SendType.Text}/{channel.Name}")
        {
            Content = JsonContent.Create(new
            {
                number = recipient.PhoneNumber.ToString(),
                text = content.Text,
                delay,
                linkPreview = true
            })
        };
    }

    private static async Task<HttpRequestMessage> CreateMediaRequest(
        CampaignContent content,
        Recipient recipient,
        Channel channel,
        EvolutionApiOptions options,
        HttpClient httpClient,
        int delay,
        CancellationToken cancellationToken)
    {
        if (content.MediaType is null)
            throw new InvalidOperationException(
                "MediaType is required for media content.");

        if (string.IsNullOrWhiteSpace(content.Url))
            throw new InvalidOperationException(
                "Url is required for media content.");

        if (string.IsNullOrWhiteSpace(content.FileName))
            throw new InvalidOperationException(
                "FileName is required for media content.");

        using HttpResponseMessage mediaResponse =
            await httpClient.GetAsync(
                content.Url,
                cancellationToken);

        mediaResponse.EnsureSuccessStatusCode();

        byte[] mediaBytes =
            await mediaResponse.Content.ReadAsByteArrayAsync(
                cancellationToken);

        ByteArrayContent media = new(mediaBytes);

        media.Headers.ContentType = new MediaTypeHeaderValue(
            content.MimeType ?? "application/octet-stream");

        MultipartFormDataContent form = new();

        form.Add(
            new StringContent(delay.ToString()),
            "delay");

        form.Add(
            new StringContent(
                recipient.PhoneNumber.ToString()),
            "number");

        form.Add(
            media,
            "media",
            content.FileName);

        if (!string.IsNullOrWhiteSpace(content.Caption))
        {
            form.Add(
                new StringContent(content.Caption),
                "caption");
        }

        form.Add(
            new StringContent(content.FileName),
            "fileName");

        return new HttpRequestMessage(
            HttpMethod.Post,
            $"{options.ApiUrl}/message/{SendType.Media}/{channel.Name}")
        {
            Content = form
        };
    }

    public static class SendType
    {
        public const string Media = "sendMedia";
        public const string Text = "sendText";
    }
}