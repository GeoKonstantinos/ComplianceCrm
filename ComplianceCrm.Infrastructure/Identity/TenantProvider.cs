using System.Security.Claims;
using ComplianceCrm.Application.Abstractions.Providers;
using Microsoft.AspNetCore.Http;

namespace ComplianceCrm.Infrastructure.Identity;

public sealed class TenantProvider(IHttpContextAccessor http) : ITenantProvider
{
    private static readonly Guid Fallback = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    public Guid GetTenantId()
    {
        var ctx = http.HttpContext;
        if (ctx?.Request.Headers.TryGetValue("X-Tenant-Id", out var hv) == true &&
            Guid.TryParse(hv.ToString(), out var id))
            return id;

        var claim = ctx?.User.FindFirst("tenant_id")?.Value;
        if (Guid.TryParse(claim, out var cid)) return cid;

        return Fallback;
    }
}
