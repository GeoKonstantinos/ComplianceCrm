
namespace ComplianceCrm.Domain.Abstractions.Interfaces;

/// <summary>
/// Provides UTC timestamps for auditing purposes.
/// </summary>
public interface ITimeStamped
{
    DateTime CreatedAtUtc { get; set; }
    DateTime? UpdatedAtUtc { get; set; }
}
