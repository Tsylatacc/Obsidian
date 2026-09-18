using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Obsidian.Domain.Entities;

namespace Obsidian.Infrastructure.Persistence.Configurations;

public sealed class SubscriptionUsageConfiguration : IEntityTypeConfiguration<SubscriptionUsage>
{
    public void Configure(EntityTypeBuilder<SubscriptionUsage> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.SubscriptionId)
            .IsRequired();

        builder.HasOne<Subscription>()
            .WithMany(x => x.SubscriptionUsages)
            .HasForeignKey(x => x.SubscriptionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.MessagesUsed)
            .IsRequired();

        builder.Property(x => x.ChannelsUsed)
            .IsRequired();

        builder.Property(x => x.TenantId)
            .HasMaxLength(100);

        builder.Property(x => x.PeriodStartedAt)
            .IsRequired();

        builder.Property(x => x.PeriodExpiresAt)
            .IsRequired();
    }
}
