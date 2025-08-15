
namespace ComplianceCrm.Application.Abstractions.Providers;

/// <summary>
/// Provides the current Tenant context for the running request/use case.
/// </summary>
public interface ITenantProvider
{
    Guid GetTenantId();
}

