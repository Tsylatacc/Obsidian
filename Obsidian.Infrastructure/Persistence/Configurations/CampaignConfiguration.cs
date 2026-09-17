using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Obsidian.Domain.Entities;

namespace Obsidian.Infrastructure.Persistence.Configurations;

public sealed class CampaignConfiguration
    : IEntityTypeConfiguration<Campaign>
{
    public void Configure(
        EntityTypeBuilder<Campaign> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ChannelId)
            .IsRequired();

        builder.HasOne(x => x.Channel)
            .WithMany()
            .HasForeignKey(x => x.ChannelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(x => x.TenantId)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.FinishedAt);

        builder.OwnsMany(
            x => x.Contents,
            content =>
            {
                content.HasKey(x => x.Position);

                content.Property(x => x.Position)
                    .IsRequired();

                content.Property(x => x.Type)
                    .HasConversion<string>()
                    .IsRequired();

                content.Property(x => x.Text)
                    .HasMaxLength(5000);

                content.Property(x => x.MediaType)
                    .HasConversion<string>();

                content.Property(x => x.MimeType)
                    .HasMaxLength(100);

                content.Property(x => x.Caption)
                    .HasMaxLength(1000);

                content.Property(x => x.Url)
                    .HasMaxLength(2000);

                content.Property(x => x.FileName)
                    .HasMaxLength(255);
            });

        builder.HasMany(x => x.Recipients)
            .WithOne()
            .HasForeignKey(x => x.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}