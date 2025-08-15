using ComplianceCrm.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplianceCrm.Application.Tasks.Dtos;

/// <summary>Request to update a task (title, type, due date, etc).</summary>
public sealed class UpdateTaskRequest
{
    public string Title { get; set; } = default!;
    public TaskType Type { get; set; }
    public DateTime DueDateUtc { get; set; }
    public string? Notes { get; set; }
}
