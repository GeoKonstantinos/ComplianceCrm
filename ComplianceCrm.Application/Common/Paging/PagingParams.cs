namespace ComplianceCrm.Application.Common.Paging;

/// <summary>
/// Standard paging parameters for list queries.
/// </summary>
public sealed record PagingParams(int Page = 1, int PageSize = 20);