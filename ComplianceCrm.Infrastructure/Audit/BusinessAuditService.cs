using ComplianceCrm.Application.Abstractions.Persistence;
using ComplianceCrm.Application.Abstractions.Providers;
using ComplianceCrm.Application.Abstractions.Services;
using ComplianceCrm.Domain.Audit;
using ComplianceCrm.Domain.Audit.Enums;

namespace ComplianceCrm.Infrastructure.Audit;

public sealed class BusinessAuditService(
    IAppDbContext db,
    ITenantProvider tenant,
    ICurrentUserService user,
    ICorrelationIdProvider corr,
    IDateTimeProvider clock) : IBusinessAuditService
{
    public async Task WriteAsync(
        TargetType targetType,
        long targetId,
        BusinessAction action,
        string? notes = null,
        string? ipAddress = null,
        CancellationToken ct = default)
    {
        var entry = new BusinessAuditLog
        {
            TenantId = tenant.GetTenantId(),
            TargetType = targetType,
            TargetId = targetId,
            Action = action,
            CorrelationId = corr.GetCorrelationId(),
            IpAddress = ipAddress,
            Notes = notes,
            CreatedAtUtc = clock.UtcNow,
            CreatedByUserId = user.GetUserId()
        };

        db.BusinessAuditLogs.Add(entry);
        await db.SaveChangesAsync(ct);
    }
}
