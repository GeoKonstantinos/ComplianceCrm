using System.Security.Claims;
using ComplianceCrm.Application.Abstractions.Providers;
using Microsoft.AspNetCore.Http;

namespace ComplianceCrm.Infrastructure.Identity;

public sealed class CurrentUserService(IHttpContextAccessor http) : ICurrentUserService
{
    public long? GetUserId()
    {
        var id = http.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                 ?? http.HttpContext?.User.FindFirst("sub")?.Value;
        return long.TryParse(id, out var l) ? l : null;
    }

    public string? GetUserName() => http.HttpContext?.User.Identity?.Name;

    public bool IsAuthenticated() => http.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
