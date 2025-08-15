namespace ComplianceCrm.Application.Tasks.Dtos;

/// <summary>Input model for creating a new task.</summary>
public sealed record CreateTaskRequest(
    long CustomerId,
    string Title,
    string? Description,
    int Type,           // TaskType
    DateTime DueDateUtc
);
