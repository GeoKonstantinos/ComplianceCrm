using ComplianceCrm.Domain.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ComplianceCrm.Infrastructure.Persistence.Configurations;

public sealed class BusinessAuditLogConfiguration : IEntityTypeConfiguration<BusinessAuditLog>
{
    public void Configure(EntityTypeBuilder<BusinessAuditLog> b)
    {
        b.ToTable("business_audit_logs");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).UseIdentityByDefaultColumn();

        b.Property(x => x.CorrelationId).IsRequired();
        b.Property(x => x.IpAddress).HasMaxLength(64);
        b.Property(x => x.Notes).HasMaxLength(1000);

        b.Property(x => x.CreatedAtUtc).HasColumnType("timestamptz");
        b.Property(x => x.UpdatedAtUtc).HasColumnType("timestamptz");

        b.HasIndex(x => new { x.TenantId, x.TargetType, x.TargetId, x.Action, x.CreatedAtUtc });
    }
}
