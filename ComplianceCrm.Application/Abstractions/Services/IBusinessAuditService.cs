using ComplianceCrm.Domain.Audit;
using ComplianceCrm.Domain.Audit.Enums;

namespace ComplianceCrm.Application.Abstractions.Services;

/// <summary>
/// Writes business-level audit entries in a centralized, consistent way.
/// Implementation will enrich with Tenant, User and Correlation.
/// </summary>
public interface IBusinessAuditService
{
    Task WriteAsync(
        TargetType targetType,
        long targetId,
        BusinessAction action,
        string? notes = null,
        string? ipAddress = null,
        CancellationToken ct = default);
}