
namespace ComplianceCrm.Application.Abstractions.Providers;

/// <summary>
/// Provides a CorrelationId used for linking logs/traces and audit entries.
/// Typically sourced from HTTP header X-Correlation-Id or OTEL traceId.
/// </summary>
public interface ICorrelationIdProvider
{
    Guid GetCorrelationId();
}