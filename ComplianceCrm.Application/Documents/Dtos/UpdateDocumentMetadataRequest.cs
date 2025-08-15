namespace ComplianceCrm.Application.Documents.Dtos;

/// <summary>Input model for updating document metadata (non-binary fields).</summary>
public sealed record UpdateDocumentMetadataRequest(
    int? Type,
    int? Sensitivity,
    bool? IsPersonalData,
    string? OriginalFileName,
    string? ContentType,
    DateTime? RetentionUntilUtc,
    bool? LegalHold,
    int? Status            // DocumentStatus
);
