using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Obsidian.Domain.Entities;

namespace Obsidian.Infrastructure.Persistence.Configurations;

public sealed class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Identifier)
            .IsRequired()
            .HasMaxLength(8);

        builder.HasIndex(x => x.Identifier)
            .IsUnique();

        builder.Property(x => x.SubscriptionPlanId)
            .IsRequired();

        builder.HasOne(x => x.SubscriptionPlan)
            .WithMany()
            .HasForeignKey(x => x.SubscriptionPlanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(x => x.TenantId)
            .HasMaxLength(100);

        builder.Property(x => x.StartedAt)
            .IsRequired();

        builder.Property(x => x.ExpiresAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired();
    }
}
