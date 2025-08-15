
namespace ComplianceCrm.Domain.Abstractions.Interfaces;

/// <summary>
/// Adds correlation ID for cross-entity or cross-service request tracing.
/// </summary>
public interface IHasCorrelation
{
    Guid CorrelationId { get; set; }
}
