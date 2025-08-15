
namespace ComplianceCrm.Domain.Abstractions.Interfaces;

/// <summary>
/// Marks an entity that belongs to a specific tenant (multi-tenant isolation).
/// </summary>
public interface ITenantScoped
{
    Guid TenantId { get; set; }
}
