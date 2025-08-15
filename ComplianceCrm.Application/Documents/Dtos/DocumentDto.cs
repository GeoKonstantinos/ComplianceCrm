namespace ComplianceCrm.Application.Documents.Dtos;

/// <summary>Read model for Document metadata.</summary>
public sealed record DocumentDto(
    long Id,
    long CustomerId,
    int Type,                 // DocumentType
    int Sensitivity,          // DocumentSensitivity
    bool IsPersonalData,
    string OriginalFileName,
    string ContentType,
    long SizeBytes,
    string Sha256,
    int Provider,             // StorageProvider
    string StoragePath,
    int Version,
    int Status,               // DocumentStatus
    DateTime? RetentionUntilUtc,
    bool LegalHold
);
