using ComplianceCrm.Domain.Abstractions;
using ComplianceCrm.Domain.Abstractions.Interfaces;

namespace ComplianceCrm.Domain.Tenants;

/// <summary>
/// Represents a tenant (organization/company) with a Guid primary key named TenantId.
/// Does not inherit from Entity to avoid a second Id.
/// </summary>
public class Tenant : ITimeStamped, IHasUserTracking
{
    /// <summary>Primary key for the tenant (globally unique).</summary>
    public Guid TenantId { get; set; }

    /// <summary>Tenant's display name (e.g., Company or Accounting Office name).</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Unique internal code for the tenant (can be used in subdomains or API keys).</summary>
    public string Code { get; set; } = string.Empty;

    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public string? Description { get; set; }

    /// <summary>Timestamps kept for consistency.</summary>
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }

    /// <summary>User tracking (optional) – can be filled at creation/update time.</summary>
    public long? CreatedByUserId { get; set; }
    public long? UpdatedByUserId { get; set; }
}