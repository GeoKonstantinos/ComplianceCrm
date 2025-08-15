namespace ComplianceCrm.Application.Tasks.Dtos;

/// <summary>Input model for updating task status.</summary>
public sealed record UpdateTaskStatusRequest(
    int Status,               // TaskStatus
    DateTime? CompletedAtUtc  // optional; set when Status == Done
);
