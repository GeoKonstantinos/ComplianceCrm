
namespace ComplianceCrm.Domain.Audit.Enums;

/// <summary>
/// Defines the type of business action that was performed on the target entity.
/// </summary>
public enum BusinessAction
{
    Unknown = 0,
    Created = 1,
    Updated = 2,
    Deleted = 3,
    StatusChanged = 4,
    Viewed = 5,
    Downloaded = 6,
    Restored = 7,
    Versioned = 8,
    AccessDenied = 9
}
