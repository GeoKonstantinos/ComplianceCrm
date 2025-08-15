using ComplianceCrm.Domain.Abstractions.Interfaces;

namespace ComplianceCrm.Domain.Abstractions;

public abstract class TenantScopedEntity : Entity, ITenantScoped
{
    /// <summary>Tenant identifier for data isolation/correlation.</summary>
    public Guid TenantId { get; set; }
}
