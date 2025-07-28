using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(p => p.ImageUri)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(p => p.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(p => p.Stock)
            .IsRequired();

        builder.Property(p => p.IsActive)
            .IsRequired();

        builder.Property(p => p.Categories)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
            )
            .HasMaxLength(500);

        // Audit fields
        builder.Property(p => p.CreatedBy)
            .HasMaxLength(100);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.LastModifiedBy)
            .HasMaxLength(100);

        builder.Property(p => p.LastModifiedAt);

        // Indexes
        builder.HasIndex(p => p.Name)
            .IsUnique();

        builder.HasIndex(p => p.IsActive);

        builder.HasIndex(p => p.CreatedAt);

        // Ignore domain events
        builder.Ignore(p => p.DomainEvents);
    }
}