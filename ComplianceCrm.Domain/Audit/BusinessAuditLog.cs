using ComplianceCrm.Domain.Abstractions;
using ComplianceCrm.Domain.Abstractions.Interfaces;
using ComplianceCrm.Domain.Audit.Enums;

namespace ComplianceCrm.Domain.Audit;

/// <summary>
/// Represents a business-level audit log entry.
/// This log records actions of business significance, regardless of entity type,
/// and is intended for accountability and compliance purposes.
/// </summary>
public class BusinessAuditLog : TenantScopedEntity, IHasCorrelation
{
    /// <summary>
    /// The type of business entity affected by the action.
    /// </summary>
    public TargetType TargetType { get; set; } = TargetType.Unknown;

    /// <summary>
    /// The unique identifier of the target entity.
    /// </summary>
    public long TargetId { get; set; }

    /// <summary>
    /// The type of business action performed.
    /// </summary>
    public BusinessAction Action { get; set; } = BusinessAction.Unknown;

    /// <summary>Correlation identifier (usually X-Correlation-Id or OTEL traceId).</summary>
    public Guid CorrelationId { get; set; }

    /// <summary>
    /// Optional IP address from which the action originated.
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// Optional short notes or contextual information describing the action.
    /// Example: "Status changed from Pending to Approved".
    /// </summary>
    public string? Notes { get; set; }
}