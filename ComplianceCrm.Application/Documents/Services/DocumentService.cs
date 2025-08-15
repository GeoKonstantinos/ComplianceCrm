using ComplianceCrm.Application.Abstractions.Persistence;
using ComplianceCrm.Application.Abstractions.Providers;
using ComplianceCrm.Application.Abstractions.Services;
using ComplianceCrm.Application.Common.Paging;
using ComplianceCrm.Application.Documents.Dtos;
using ComplianceCrm.Domain.Audit;
using ComplianceCrm.Domain.Audit.Enums;
using ComplianceCrm.Domain.Documents;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ComplianceCrm.Application.Documents.Services;

/// <summary>
/// Implements document metadata use cases (binary IO is handled in Infrastructure).
/// </summary>
public sealed class DocumentService(
    IAppDbContext db,
    ITenantProvider tenant,
    ICurrentUserService currentUser,
    IDateTimeProvider clock,
    IBusinessAuditService audit,
    IValidator<CreateDocumentRequest> createValidator,
    IValidator<UpdateDocumentMetadataRequest> updateValidator,
    IValidator<PagingParams> pagingValidator) : IDocumentService
{
    private readonly IAppDbContext _db = db;
    private readonly ITenantProvider _tenant = tenant;
    private readonly ICurrentUserService _currentUser = currentUser;
    private readonly IDateTimeProvider _clock = clock;
    private readonly IBusinessAuditService _audit = audit;
    private readonly IValidator<CreateDocumentRequest> _createValidator = createValidator;
    private readonly IValidator<UpdateDocumentMetadataRequest> _updateValidator = updateValidator;
    private readonly IValidator<PagingParams> _pagingValidator = pagingValidator;

    public async Task<IReadOnlyList<DocumentDto>> ListAsync(DocumentQuery query, CancellationToken ct)
    {
        await _pagingValidator.ValidateAndThrowAsync(query.Paging, ct);

        var tenantId = _tenant.GetTenantId();

        var q = _db.Documents
            .Where(d => d.TenantId == tenantId);

        if (query.CustomerId.HasValue)
            q = q.Where(d => d.CustomerId == query.CustomerId.Value);

        if (query.Type.HasValue)
            q = q.Where(d => d.Type == query.Type.Value);

        if (query.Status.HasValue)
            q = q.Where(d => d.Status == query.Status.Value);

        if (query.CreatedFromUtc.HasValue)
            q = q.Where(d => d.CreatedAtUtc >= query.CreatedFromUtc.Value);

        if (query.CreatedToUtc.HasValue)
            q = q.Where(d => d.CreatedAtUtc <= query.CreatedToUtc.Value);

        var items = await q
            .OrderByDescending(d => d.CreatedAtUtc)
            .Skip((query.Paging.Page - 1) * query.Paging.PageSize)
            .Take(query.Paging.PageSize)
            .AsNoTracking()
            .Select(d => new DocumentDto(
                d.Id, d.CustomerId,
                (int)d.Type, (int)d.Sensitivity, d.IsPersonalData,
                d.OriginalFileName, d.ContentType, d.SizeBytes, d.Sha256,
                (int)d.Provider, d.StoragePath, d.Version, (int)d.Status,
                d.RetentionUntilUtc, d.LegalHold))
            .ToListAsync(ct);

        return items;
    }
    public async Task<IReadOnlyList<DocumentDto>> ListByCustomerAsync(long customerId, PagingParams paging, CancellationToken ct = default)
    {
        await _pagingValidator.ValidateAndThrowAsync(paging, ct);
        var tenantId = _tenant.GetTenantId();

        var exists = await _db.Customers.AnyAsync(c => c.TenantId == tenantId && c.Id == customerId, ct);
        if (!exists) throw new KeyNotFoundException("Customer not found for current tenant.");

        var items = await _db.Documents
            .Where(d => d.TenantId == tenantId && d.CustomerId == customerId)
            .OrderByDescending(d => d.CreatedAtUtc)
            .Skip((paging.Page - 1) * paging.PageSize)
            .Take(paging.PageSize)
            .AsNoTracking()
            .Select(d => new DocumentDto(
                d.Id, d.CustomerId,
                (int)d.Type, (int)d.Sensitivity, d.IsPersonalData,
                d.OriginalFileName, d.ContentType, d.SizeBytes, d.Sha256,
                (int)d.Provider, d.StoragePath, d.Version, (int)d.Status,
                d.RetentionUntilUtc, d.LegalHold))
            .ToListAsync(ct);

        return items;
    }

    public async Task<long> CreateAsync(CreateDocumentRequest request, CancellationToken ct = default)
    {
        await _createValidator.ValidateAndThrowAsync(request, ct);

        var tenantId = _tenant.GetTenantId();
        var now = _clock.UtcNow;
        var userId = _currentUser.GetUserId();

        // Guard: customer exists in tenant
        var exists = await _db.Customers.AnyAsync(c => c.TenantId == tenantId && c.Id == request.CustomerId, ct);
        if (!exists) throw new KeyNotFoundException("Customer not found for current tenant.");

        var entity = new Document
        {
            TenantId = tenantId,
            CustomerId = request.CustomerId,
            Type = (DocumentType)request.Type,
            Sensitivity = (DocumentSensitivity)request.Sensitivity,
            IsPersonalData = request.IsPersonalData,
            OriginalFileName = request.OriginalFileName.Trim(),
            ContentType = request.ContentType.Trim(),
            SizeBytes = request.SizeBytes,
            Sha256 = request.Sha256.ToLowerInvariant(),
            Provider = (StorageProvider)request.Provider,
            StoragePath = request.StoragePath.Trim(),
            Version = 1,
            Status = DocumentStatus.Active,
            RetentionUntilUtc = request.RetentionUntilUtc,
            LegalHold = request.LegalHold,
            CreatedAtUtc = now,
            CreatedByUserId = userId
        };

        _db.Documents.Add(entity);
        await _db.SaveChangesAsync(ct);

        await _audit.WriteAsync(TargetType.Document, entity.Id, BusinessAction.Created, "Document metadata created", ct: ct);

        return entity.Id;
    }

    public async Task UpdateMetadataAsync(long documentId, UpdateDocumentMetadataRequest request, CancellationToken ct = default)
    {
        await _updateValidator.ValidateAndThrowAsync(request, ct);

        var tenantId = _tenant.GetTenantId();
        var now = _clock.UtcNow;
        var userId = _currentUser.GetUserId();

        var entity = await _db.Documents
            .FirstOrDefaultAsync(d => d.TenantId == tenantId && d.Id == documentId, ct) 
            ?? throw new KeyNotFoundException("Document not found for current tenant.");

        // Apply patch-like updates
        if (request.Type.HasValue) entity.Type = (DocumentType)request.Type.Value;
        if (request.Sensitivity.HasValue) entity.Sensitivity = (DocumentSensitivity)request.Sensitivity.Value;
        if (request.IsPersonalData.HasValue) entity.IsPersonalData = request.IsPersonalData.Value;
        if (request.OriginalFileName is { Length: > 0 }) entity.OriginalFileName = request.OriginalFileName.Trim();
        if (request.ContentType is { Length: > 0 }) entity.ContentType = request.ContentType.Trim();
        if (request.RetentionUntilUtc.HasValue) entity.RetentionUntilUtc = request.RetentionUntilUtc.Value;
        if (request.LegalHold.HasValue) entity.LegalHold = request.LegalHold.Value;
        if (request.Status.HasValue) entity.Status = (DocumentStatus)request.Status.Value;

        entity.UpdatedAtUtc = now;
        entity.UpdatedByUserId = userId;

        await _db.SaveChangesAsync(ct);

        await _audit.WriteAsync(TargetType.Document, entity.Id, BusinessAction.Updated, "Document metadata updated", ct: ct);
    }

    public async Task UpdateStatusAsync(long id, UpdateDocumentStatusRequest request, CancellationToken ct)
    {
        var tenantId = _tenant.GetTenantId();

        var doc = await _db.Documents
            .Where(d => d.TenantId == tenantId && d.Id == id)
            .FirstOrDefaultAsync(ct)
            ?? throw new KeyNotFoundException($"Document {id} not found.");

        doc.Status = request.Status;
        doc.UpdatedAtUtc = _clock.UtcNow;

        await _db.SaveChangesAsync(ct);

        await _audit.WriteAsync(TargetType.Document, id, BusinessAction.Updated, "Status updated", ct: ct);
    }

    public async Task DeleteAsync(long id, CancellationToken ct)
    {
        var tenantId = _tenant.GetTenantId();

        var doc = await _db.Documents
            .Where(d => d.TenantId == tenantId && d.Id == id)
            .FirstOrDefaultAsync(ct)
            ?? throw new KeyNotFoundException($"Document {id} not found.");

        _db.Documents.Remove(doc);
        await _db.SaveChangesAsync(ct);

        await _audit.WriteAsync(TargetType.Document, id, BusinessAction.Deleted, "Document deleted", ct: ct);
    }
}
