using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Obsidian.Domain.Entities;

namespace Obsidian.Infrastructure.Persistence.Configurations;

public sealed class ChannelConfiguration : IEntityTypeConfiguration<Channel>
{
    public void Configure(EntityTypeBuilder<Channel> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Token)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(x => x.Integration)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.TenantId)
            .HasMaxLength(100);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired();

        builder.OwnsOne(x => x.PhoneNumber, phoneNumber =>
        {
            phoneNumber.Property(x => x.Value)
                .HasColumnName("phone_number")
                .IsRequired()
                .HasMaxLength(30);
        });

        builder.OwnsOne(x => x.QRCode, qrCode =>
        {
            qrCode.Property(x => x.PairingCode)
                .HasMaxLength(100);

            qrCode.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(500);
        });
    }
}
