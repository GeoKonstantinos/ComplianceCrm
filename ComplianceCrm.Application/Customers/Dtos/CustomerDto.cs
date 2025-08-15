namespace ComplianceCrm.Application.Customers.Dtos;

/// <summary>Read model for Customer data exposed to UI/API.</summary>
public record CustomerDto(
    long Id,
    string Name,
    string Email,
    string? Phone,
    bool Consented,
    DateTime? ConsentDateUtc
);
