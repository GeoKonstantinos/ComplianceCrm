using ComplianceCrm.Application.Abstractions.Providers;
using Microsoft.AspNetCore.Http;

namespace ComplianceCrm.Infrastructure.Identity;

public sealed class CorrelationIdProvider(IHttpContextAccessor http) : ICorrelationIdProvider
{
    public Guid GetCorrelationId()
    {
        var ctx = http.HttpContext;
        if (ctx?.Request.Headers.TryGetValue("X-Correlation-Id", out var hv) == true &&
            Guid.TryParse(hv.ToString(), out var id))
            return id;

        return Guid.NewGuid();
    }
}
