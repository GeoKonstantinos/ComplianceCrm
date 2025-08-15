
namespace ComplianceCrm.Domain.Abstractions.Interfaces;

/// <summary>
/// Adds CreatedBy/UpdatedBy tracking (user auditing).
/// </summary>
public interface IHasUserTracking
{
    long? CreatedByUserId { get; set; }
    long? UpdatedByUserId { get; set; }
}
