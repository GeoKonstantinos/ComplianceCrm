namespace ComplianceCrm.Application.Customers.Dtos;

/// <summary>Input model for creating a new Customer.</summary>
public record CreateCustomerRequest(
    string Name,
    string Email,
    string? Phone,
    bool Consented
);
