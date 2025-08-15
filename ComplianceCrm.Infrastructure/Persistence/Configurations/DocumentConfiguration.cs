using ComplianceCrm.Domain.Documents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ComplianceCrm.Infrastructure.Persistence.Configurations;

public sealed class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> b)
    {
        b.ToTable("documents");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).UseIdentityByDefaultColumn();

        b.Property(x => x.OriginalFileName).HasMaxLength(255).IsRequired();
        b.Property(x => x.ContentType).HasMaxLength(128).IsRequired();
        b.Property(x => x.StoragePath).HasMaxLength(500).IsRequired();
        b.Property(x => x.Sha256).HasMaxLength(64).IsRequired();

        b.Property(x => x.CreatedAtUtc).HasColumnType("timestamptz");
        b.Property(x => x.UpdatedAtUtc).HasColumnType("timestamptz");
        b.Property(x => x.RetentionUntilUtc).HasColumnType("timestamptz");

        b.HasOne(x => x.Customer)
            .WithMany(c => c.Documents)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => new { x.TenantId, x.CustomerId, x.CreatedAtUtc });
        b.HasIndex(x => new { x.TenantId, x.Sha256 });
    }
}
