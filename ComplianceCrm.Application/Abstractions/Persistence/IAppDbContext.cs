using ComplianceCrm.Domain.Audit;
using ComplianceCrm.Domain.Customers;
using ComplianceCrm.Domain.Documents;
using Microsoft.EntityFrameworkCore;

namespace ComplianceCrm.Application.Abstractions.Persistence;

/// <summary>
/// Application-facing contract for persistence (EF-agnostic).
/// Implemented in Infrastructure by the actual DbContext.
/// </summary>
public interface IAppDbContext
{
    DbSet<Customer> Customers { get; }
    DbSet<Domain.Customers.Task> Tasks { get; }
    DbSet<Document> Documents { get; }
    DbSet<BusinessAuditLog> BusinessAuditLogs { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}