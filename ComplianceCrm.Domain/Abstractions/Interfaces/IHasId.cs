
namespace ComplianceCrm.Domain.Abstractions.Interfaces;

/// <summary>
/// Marker interface for entities that expose a long primary key.
/// </summary>
public interface IHasId
{
    long Id { get; set; }
}
