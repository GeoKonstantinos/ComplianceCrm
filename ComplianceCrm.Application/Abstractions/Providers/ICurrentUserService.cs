
namespace ComplianceCrm.Application.Abstractions.Providers;

/// <summary>
/// Provides information about the authenticated user.
/// </summary>
public interface ICurrentUserService
{
    long? GetUserId();
    string? GetUserName();
    bool IsAuthenticated();
}