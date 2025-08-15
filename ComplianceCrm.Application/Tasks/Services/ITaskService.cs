using ComplianceCrm.Application.Common.Paging;
using ComplianceCrm.Application.Tasks.Dtos;

namespace ComplianceCrm.Application.Tasks.Services;

public interface ITaskService
{
    Task<IReadOnlyList<TaskDto>> ListAsync(TaskQuery query, CancellationToken ct = default);
    Task<long> CreateAsync(CreateTaskRequest request, CancellationToken ct = default);
    Task UpdateStatusAsync(long taskId, UpdateTaskStatusRequest request, CancellationToken ct = default);
    Task UpdateAsync(long id, UpdateTaskRequest req, CancellationToken ct);
    Task DeleteAsync(long id, CancellationToken ct);
    Task RescheduleAsync(long id, DateTime startUtc, DateTime? endUtc, CancellationToken ct);
}
