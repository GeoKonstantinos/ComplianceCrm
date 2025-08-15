using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplianceCrm.Application.Tasks.Dtos;

/// <summary>
/// Request to reschedule a task in UTC. If EndDateUtc is null, the service may infer a default duration.
/// </summary>
public sealed class RescheduleTaskRequest
{
    /// <summary>New start in UTC.</summary>
    public DateTime StartDateUtc { get; init; }

    /// <summary>New end in UTC. Optional; if null, the service may set a default end.</summary>
    public DateTime? EndDateUtc { get; init; }
}
