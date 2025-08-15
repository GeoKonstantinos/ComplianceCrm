
namespace ComplianceCrm.Application.Abstractions.Providers;

/// <summary>
/// Provides UTC date/time so logic is testable and timezone-agnostic.
/// </summary>
public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
