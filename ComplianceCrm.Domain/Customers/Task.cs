using ComplianceCrm.Domain.Abstractions;

namespace ComplianceCrm.Domain.Customers;

public enum TaskType { Tax = 1, Insurance = 2, ESG = 3, Other = 99 }
public enum TaskStatus { Pending = 1, InProgress = 2, Done = 3, Overdue = 4 }

/// <summary>
/// A task/obligation belonging to a customer.
/// Designed for calendar visualization and SLA tracking.
/// </summary>
public class Task : TenantScopedEntity
{
    public long CustomerId { get; set; }
    public Customer Customer { get; set; } = default!;

    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public TaskType Type { get; set; }
    public TaskStatus Status { get; set; } = TaskStatus.Pending;

    public DateTime DueDateUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
}
