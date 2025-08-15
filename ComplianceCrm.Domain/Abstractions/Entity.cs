using ComplianceCrm.Domain.Abstractions.Interfaces;

namespace ComplianceCrm.Domain.Abstractions;

/// <summary>
/// Base class for domain entities that use a long identity key and include UTC timestamps.
/// Keep this base small; add capabilities via interfaces.
/// </summary>
public abstract class Entity : IHasId, ITimeStamped, IHasUserTracking
{
    /// <summary>Database-generated primary key (BIGINT/IDENTITY).</summary>
    public long Id { get; set; }

    /// <summary>UTC timestamp when the row was created.</summary>
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    /// <summary>UTC timestamp when the row was last updated.</summary>
    public DateTime? UpdatedAtUtc { get; set; }
    public long? CreatedByUserId { get; set; }
    public long? UpdatedByUserId { get; set; }
}
