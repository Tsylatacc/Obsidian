using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Obsidian.Domain.Entities;

namespace Obsidian.Infrastructure.Persistence.Configurations;

public sealed class CampaignConfiguration : IEntityTypeConfiguration<Campaign>
{
    public void Configure(EntityTypeBuilder<Campaign> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Message)
            .IsRequired()
            .HasMaxLength(5000);

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

        builder.OwnsOne(x => x.Media, media =>
        {
            media.Property(x => x.Type)
                .HasConversion<string>()
                .IsRequired();

            media.Property(x => x.MimeType)
                .IsRequired()
                .HasMaxLength(100);

            media.Property(x => x.Caption)
                .HasMaxLength(1000);

            media.Property(x => x.Url)
                .IsRequired()
                .HasMaxLength(2000);

            media.Property(x => x.FileName)
                .IsRequired()
                .HasMaxLength(255);
        });

        builder.HasMany(x => x.Recipients)
            .WithOne()
            .HasForeignKey(x => x.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}