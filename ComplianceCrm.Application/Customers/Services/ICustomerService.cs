using ComplianceCrm.Application.Customers.Dtos;

namespace ComplianceCrm.Application.Customers.Services;

/// <summary>
/// Customer application service (use cases).
/// </summary>
public interface ICustomerService
{
    Task<IReadOnlyList<CustomerDto>> ListAsync(int page, int pageSize, CancellationToken ct = default);
    Task<long> CreateAsync(CreateCustomerRequest request, CancellationToken ct = default);
}