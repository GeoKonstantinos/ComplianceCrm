using ComplianceCrm.Domain.Abstractions;
using ComplianceCrm.Domain.Customers;

namespace ComplianceCrm.Domain.Documents;

public enum DocumentType
{
    Unknown = 0,
    Contract = 1,
    Invoice = 2,
    Receipt = 3,
    IdDocument = 4,
    TaxReturn = 5,
    InsurancePolicy = 6,
    ESGReport = 7,
    Other = 99
}

public enum DocumentSensitivity
{
    Low = 0,
    Medium = 1,
    High = 2
}

public enum StorageProvider
{
    Local = 1,
    UNC = 2,
    S3 = 3,
    AzureBlob = 4
}

public enum DocumentStatus
{
    Active = 1,
    Archived = 2,
    Deleted = 3
}

/// <summary>
/// Document metadata (provider-agnostic).
/// Το πραγματικό binary ζει σε εξωτερικό storage (Local/UNC/S3/Azure).
/// </summary>
public class Document : TenantScopedEntity
{
    public long CustomerId { get; set; }
    public Customer Customer { get; set; } = default!;

    // Classification
    public DocumentType Type { get; set; } = DocumentType.Unknown;
    public DocumentSensitivity Sensitivity { get; set; } = DocumentSensitivity.Low;
    public bool IsPersonalData { get; set; } = true;

    // File metadata
    public string OriginalFileName { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public long SizeBytes { get; set; }
    public string Sha256 { get; set; } = default!;   // hex string

    // Storage
    public StorageProvider Provider { get; set; } = StorageProvider.Local;
    public string StoragePath { get; set; } = default!; // relative path or URL
    public int Version { get; set; } = 1;
    public DocumentStatus Status { get; set; } = DocumentStatus.Active;

    // Retention / legal
    public DateTime? RetentionUntilUtc { get; set; }
    public bool LegalHold { get; set; } = false;

}