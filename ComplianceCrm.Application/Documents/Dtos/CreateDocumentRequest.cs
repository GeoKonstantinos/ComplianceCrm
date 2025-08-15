namespace ComplianceCrm.Application.Documents.Dtos;

/// <summary>Input model for creating a document metadata entry.</summary>
public sealed record CreateDocumentRequest(
    long CustomerId,
    int Type,                 // DocumentType
    int Sensitivity,          // DocumentSensitivity
    bool IsPersonalData,
    string OriginalFileName,
    string ContentType,
    long SizeBytes,
    string Sha256,            // hex string (64 chars for SHA-256)
    int Provider,             // StorageProvider
    string StoragePath,
    DateTime? RetentionUntilUtc,
    bool LegalHold
);
