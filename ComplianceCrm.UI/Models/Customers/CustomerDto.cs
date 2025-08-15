namespace ComplianceCrm.UI.Models.Customers;

public sealed class CustomerDto
{
    public long Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public bool Consented { get; set; }
    public DateTime? ConsentDateUtc { get; set; }
}
