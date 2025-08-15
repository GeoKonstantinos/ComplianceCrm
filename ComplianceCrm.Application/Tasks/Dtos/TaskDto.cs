namespace ComplianceCrm.Application.Tasks.Dtos;

/// <summary>Read model for CustomerTask.</summary>
public sealed record TaskDto(
    long Id,
    long CustomerId,
    string Title,
    string? Description,
    int Type,        // maps to TaskType enum
    int Status,      // maps to TaskStatus enum
    DateTime DueDateUtc,
    DateTime? CompletedAtUtc
);
