using ComplianceCrm.Application.Common.Paging;
using ComplianceCrm.Application.Documents.Dtos;

namespace ComplianceCrm.Application.Documents.Services;

public interface IDocumentService
{
    Task<IReadOnlyList<DocumentDto>> ListAsync(DocumentQuery query, CancellationToken ct);
    Task<IReadOnlyList<DocumentDto>> ListByCustomerAsync(long customerId, PagingParams paging, CancellationToken ct = default);
    Task<long> CreateAsync(CreateDocumentRequest request, CancellationToken ct = default);
    Task UpdateMetadataAsync(long documentId, UpdateDocumentMetadataRequest request, CancellationToken ct = default);
    Task UpdateStatusAsync(long id, UpdateDocumentStatusRequest request, CancellationToken ct);
    Task DeleteAsync(long id, CancellationToken ct);
}
