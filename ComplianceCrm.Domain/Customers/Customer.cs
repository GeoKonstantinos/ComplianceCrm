using ComplianceCrm.Domain.Abstractions;
using ComplianceCrm.Domain.Documents;

namespace ComplianceCrm.Domain.Customers;

/// <summary>
/// Customer aggregate root with basic GDPR fields.
/// </summary>
public class Customer : TenantScopedEntity
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? Phone { get; set; }

    // GDPR consent
    public bool Consented { get; set; }
    public DateTime? ConsentDateUtc { get; set; }

    // Navigation
    public ICollection<Task> Tasks { get; set; } = [];
    public ICollection<Document> Documents { get; set; } = [];

}
