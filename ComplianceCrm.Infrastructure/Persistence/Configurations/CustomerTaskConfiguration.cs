using ComplianceCrm.Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ComplianceCrm.Infrastructure.Persistence.Configurations;

public sealed class CustomerTaskConfiguration : IEntityTypeConfiguration<Domain.Customers.Task>
{
    public void Configure(EntityTypeBuilder<Domain.Customers.Task> b)
    {
        b.ToTable("customer_tasks");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).UseIdentityByDefaultColumn();
        b.Property(x => x.Title).HasMaxLength(200).IsRequired();

        b.Property(x => x.CreatedAtUtc).HasColumnType("timestamptz");
        b.Property(x => x.UpdatedAtUtc).HasColumnType("timestamptz");
        b.Property(x => x.CompletedAtUtc).HasColumnType("timestamptz");

        b.HasOne(x => x.Customer)
            .WithMany(c => c.Tasks)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => new { x.TenantId, x.DueDateUtc });
    }
}
