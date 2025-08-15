using ComplianceCrm.Application.Abstractions.Providers;
using ComplianceCrm.Domain.Abstractions;
using ComplianceCrm.Domain.Abstractions.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ComplianceCrm.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Auto-sets CreatedAt/UpdatedAt and CreatedBy/UpdatedBy on tracked entities.
/// </summary>
public sealed class AuditStampInterceptor(IDateTimeProvider clock, ICurrentUserService currentUser) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        Apply(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        Apply(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void Apply(DbContext? ctx)
    {
        if (ctx is null) return;

        var now = clock.UtcNow;
        var userId = currentUser.GetUserId();

        foreach (var e in ctx.ChangeTracker.Entries())
        {
            if (e.Entity is ITimeStamped ts)
            {
                if (e.State == EntityState.Added) ts.CreatedAtUtc = now;
                if (e.State == EntityState.Modified) ts.UpdatedAtUtc = now;
            }
            if (e.Entity is IHasUserTracking ut)
            {
                if (e.State == EntityState.Added) ut.CreatedByUserId = userId;
                if (e.State == EntityState.Modified) ut.UpdatedByUserId = userId;
            }
        }
    }
}
