
using ComplianceCrm.Application.Common.Paging;
using ComplianceCrm.Domain.Documents;

namespace ComplianceCrm.Application.Documents.Dtos;

/// <summary>Query parameters for searching documents.</summary>
public sealed record DocumentQuery(
    PagingParams Paging,
    long? CustomerId = null,
    DocumentType? Type = null,
    DocumentStatus? Status = null,
    DateTime? CreatedFromUtc = null,
    DateTime? CreatedToUtc = null
);