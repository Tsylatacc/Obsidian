using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Obsidian.Domain.Entities;
using Obsidian.Domain.Enums;
using Obsidian.Domain.ValueObjects;
using Obsidian.Infrastructure.Persistence;
using Wolverine;

namespace Obsidian.Application.Features
{
    public class CampaignHandler
    {
        public static async Task Handle(
             CampaignCommand command,
             ObsidianDbContext db,
             IMessageBus bus,
             CancellationToken cancellationToken)
        {
            if (!await db.Channels.AnyAsync(x => x.Id == command.ChannelId, cancellationToken))
                throw new KeyNotFoundException($"Channel {command.ChannelId} not found.");

            CampaignMedia? media = null;

            if (command.Media is not null)
            {
                FileExtensionContentTypeProvider provider = new();

                if (!provider.TryGetContentType(
                        command.Media.FileName,
                        out string? mimeType))
                {
                    throw new ArgumentException("Invalid file type.");
                }

                media = CampaignMedia.Create(
                    command.Media.Type,
                    mimeType,
                    command.Media.Caption,
                    command.Media.Url,
                    command.Media.FileName);
            }

            if (command.PhoneNumbers.Count == 0)
                throw new ArgumentException("At least one phone number is required.");

            if (command.PhoneNumbers.Count > 10000)
                throw new ArgumentException("Maximum of 10,000 phone numbers.");


            Campaign campaign = Campaign.Create(command.Message, command.ChannelId);

            List<Recipient> recipients = [];
            foreach (string phoneNumber in command.PhoneNumbers.Distinct())
            {
                recipients.Add(Recipient.Create(campaign.Id, phoneNumber));
            }

            campaign.SetRecipients(recipients);

            await db.Campaigns.AddAsync(campaign, cancellationToken);
            await bus.PublishAsync(new CampaignRequested(campaign.Id, campaign.ChannelId));
        }
    }
    public sealed record CampaignCommand(
        IReadOnlyCollection<string> PhoneNumbers,
        string Message,
        MediaRequest? Media,
        Guid ChannelId
    );

    public sealed record MediaRequest(
        MediaType Type,
        string? Caption,
        string Url,
        string FileName
    );

    public sealed record CampaignRequested(
        Guid CampaignId,
        Guid ChannelId);
}
