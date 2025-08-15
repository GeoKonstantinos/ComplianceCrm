using ComplianceCrm.Application.Abstractions.Persistence;
using ComplianceCrm.Application.Abstractions.Providers;
using ComplianceCrm.Domain.Audit;
using ComplianceCrm.Domain.Customers;
using ComplianceCrm.Domain.Documents;
using ComplianceCrm.Domain.Tenants;
using Microsoft.EntityFrameworkCore;

namespace ComplianceCrm.Infrastructure.Persistence;

/// <summary>
/// EF Core DbContext implementing IAppDbContext.
/// Applies snake_case naming, default schema, and global Tenant query filters.
/// </summary>
public sealed class AppDbContext(DbContextOptions<AppDbContext> options, ITenantProvider tenantProvider) : DbContext(options), IAppDbContext
{

    // Exposed for query filters
    internal Guid CurrentTenantId => tenantProvider.GetTenantId();

    // IAppDbContext sets
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Domain.Customers.Task> Tasks => Set<Domain.Customers.Task>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<BusinessAuditLog> BusinessAuditLogs => Set<BusinessAuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Postgres-friendly naming & schema
        modelBuilder.HasDefaultSchema("crm");
        // Apply configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Global multi-tenant filters
        modelBuilder.Entity<Customer>().HasQueryFilter(x => x.TenantId == CurrentTenantId);
        modelBuilder.Entity<Domain.Customers.Task>().HasQueryFilter(x => x.TenantId == CurrentTenantId);
        modelBuilder.Entity<Document>().HasQueryFilter(x => x.TenantId == CurrentTenantId);
        modelBuilder.Entity<BusinessAuditLog>().HasQueryFilter(x => x.TenantId == CurrentTenantId);
    }

    internal void ApplyTenantFilter<TEntity>(ModelBuilder modelBuilder) where TEntity : class
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e =>
            EF.Property<Guid>(e, "TenantId") == CurrentTenantId);
    }
}
