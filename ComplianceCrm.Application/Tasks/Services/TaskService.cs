using ComplianceCrm.Application.Abstractions.Persistence;
using ComplianceCrm.Application.Abstractions.Providers;
using ComplianceCrm.Application.Abstractions.Services;
using ComplianceCrm.Application.Common.Paging;
using ComplianceCrm.Application.Tasks.Dtos;
using ComplianceCrm.Domain.Audit.Enums;
using ComplianceCrm.Domain.Customers;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ComplianceCrm.Application.Tasks.Services;

/// <summary>
/// Implements task-related use cases with tenant/user/correlation-aware auditing.
/// </summary>
public sealed class TaskService(
    IAppDbContext db,
    ITenantProvider tenant,
    ICurrentUserService currentUser,
    IDateTimeProvider clock,
    IBusinessAuditService audit,
    IValidator<CreateTaskRequest> createValidator,
    IValidator<UpdateTaskStatusRequest> statusValidator,
    IValidator<PagingParams> pagingValidator) : ITaskService
{
    public async Task<IReadOnlyList<TaskDto>> ListAsync(TaskQuery query, CancellationToken ct = default)
    {
        await pagingValidator.ValidateAndThrowAsync(query.Paging, ct);
        var tenantId = tenant.GetTenantId();

        var q = db.Tasks
            .Where(t => t.TenantId == tenantId);

        if (query.CustomerId.HasValue && query.CustomerId.Value > 0)
            q = q.Where(t => t.CustomerId == query.CustomerId.Value);

        if (query.Type.HasValue)
            q = q.Where(t => (int)t.Type == (int)query.Type.Value);

        if (query.Status.HasValue)
            q = q.Where(t => (int)t.Status == (int)query.Status.Value);

        var items = await q
            .OrderBy(t => t.DueDateUtc)
            .Skip((query.Paging.Page - 1) * query.Paging.PageSize)
            .Take(query.Paging.PageSize)
            .AsNoTracking()
            .Select(t => new TaskDto(
                t.Id, t.CustomerId, t.Title, t.Description,
                (int)t.Type, (int)t.Status, t.DueDateUtc, t.CompletedAtUtc))
            .ToListAsync(ct);

        return items;
    }

    public async Task<long> CreateAsync(CreateTaskRequest request, CancellationToken ct = default)
    {
        await createValidator.ValidateAndThrowAsync(request, ct);

        var tenantId = tenant.GetTenantId();
        var now = clock.UtcNow;
        var userId = currentUser.GetUserId();

        // Guard: customer must exist in the same tenant
        var exists = await db.Customers
            .AnyAsync(c => c.TenantId == tenantId && c.Id == request.CustomerId, ct);
        if (!exists)
            throw new KeyNotFoundException("Customer not found for current tenant.");

        var entity = new Domain.Customers.Task
        {
            TenantId = tenantId,
            CustomerId = request.CustomerId,
            Title = request.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            Type = (TaskType)request.Type,
            Status = Domain.Customers.TaskStatus.Pending,
            DueDateUtc = request.DueDateUtc,
            CreatedAtUtc = now,
            CreatedByUserId = userId
        };

        db.Tasks.Add(entity);
        await db.SaveChangesAsync(ct);

        await audit.WriteAsync(TargetType.Task, entity.Id, BusinessAction.Created, "Task created", ct: ct);

        return entity.Id;
    }

    public async System.Threading.Tasks.Task UpdateStatusAsync(long taskId, UpdateTaskStatusRequest request, CancellationToken ct = default)
    {
        await statusValidator.ValidateAndThrowAsync(request, ct);

        var tenantId = tenant.GetTenantId();
        var now = clock.UtcNow;
        var userId = currentUser.GetUserId();

        var entity = await db.Tasks
            .FirstOrDefaultAsync(t => t.TenantId == tenantId && t.Id == taskId, ct) 
            ?? throw new KeyNotFoundException("Task not found for current tenant.");
        
        var old = entity.Status;
        entity.Status = (Domain.Customers.TaskStatus)request.Status;
        entity.CompletedAtUtc = request.Status == 3 ? (request.CompletedAtUtc ?? now) : null; // 3 == Done
        entity.UpdatedAtUtc = now;
        entity.UpdatedByUserId = userId;

        await db.SaveChangesAsync(ct);

        await audit.WriteAsync(
            TargetType.Task, entity.Id, BusinessAction.StatusChanged,
            notes: $"{old} -> {entity.Status}", ct: ct);
    }

    public async System.Threading.Tasks.Task UpdateAsync(long id, UpdateTaskRequest req, CancellationToken ct)
    {
        var tenantId = tenant.GetTenantId();

        var task = await db.Tasks
            .Where(t => t.TenantId == tenantId && t.Id == id)
            .FirstOrDefaultAsync(ct)
            ?? throw new KeyNotFoundException($"Task {id} not found.");

        task.Title = req.Title;
        task.Type = req.Type;
        task.DueDateUtc = req.DueDateUtc;
        task.UpdatedAtUtc = clock.UtcNow;

        await db.SaveChangesAsync(ct);
        await audit.WriteAsync(TargetType.Task, id, BusinessAction.Updated, "Task updated", ct: ct);
    }

    public async System.Threading.Tasks.Task DeleteAsync(long id, CancellationToken ct)
    {
        var tenantId = tenant.GetTenantId();

        var task = await db.Tasks
            .Where(t => t.TenantId == tenantId && t.Id == id)
            .FirstOrDefaultAsync(ct)
            ?? throw new KeyNotFoundException($"Task {id} not found.");

        db.Tasks.Remove(task);
        await db.SaveChangesAsync(ct);

        await audit.WriteAsync(TargetType.Task, id, BusinessAction.Deleted, "Task deleted", ct: ct);
    }

    public async System.Threading.Tasks.Task RescheduleAsync(long id, DateTime startUtc, DateTime? endUtc, CancellationToken ct)
    {
        var tenantId = tenant.GetTenantId();

        var task = await db.Tasks
            .Where(t => t.TenantId == tenantId && t.Id == id)
            .FirstOrDefaultAsync(ct);

        if (task is null)
            throw new KeyNotFoundException($"Task {id} not found");

        // ✅ Αν το entity σου έχει Start/End:
        // task.StartDateUtc = startUtc;
        // task.EndDateUtc = endUtc ?? startUtc.AddHours(1);

        // ✅ Αν έχεις μόνο DueDateUtc στο entity, χρησιμοποίησε αυτό:
         task.DueDateUtc = startUtc;

        task.UpdatedAtUtc = clock.UtcNow;

        await db.SaveChangesAsync(ct);

        // (Προαιρετικό) Audit log:
        // await _audit.LogAsync(BusinessAction.Rescheduled, TargetType.Task, task.Id, ...);
    }
}
