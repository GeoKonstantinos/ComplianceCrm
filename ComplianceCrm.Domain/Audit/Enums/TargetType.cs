
namespace ComplianceCrm.Domain.Audit.Enums;

/// <summary>
/// Defines the type of business entity that is the target of the audit log.
/// </summary>
public enum TargetType
{
    Unknown = 0,
    Customer = 1,
    Task = 2,
    Document = 3,
}
