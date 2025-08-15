using ComplianceCrm.Application.Abstractions.Providers;

namespace ComplianceCrm.Infrastructure.Identity;

public sealed class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
