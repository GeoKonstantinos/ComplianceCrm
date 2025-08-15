using ComplianceCrm.Application.Common.Paging;
using ComplianceCrm.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplianceCrm.Application.Tasks.Dtos;

/// <summary>Query filters for searching tasks.</summary>
public sealed record TaskQuery(
    PagingParams Paging,
    long? CustomerId = null,
    TaskType? Type = null,
    Domain.Customers.TaskStatus? Status = null,
    DateTime? DueFromUtc = null,
    DateTime? DueToUtc = null
);