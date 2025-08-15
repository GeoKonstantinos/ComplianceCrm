using ComplianceCrm.Application.Abstractions.Providers;

namespace ComplianceCrm.Api.Support;

/// <summary>
/// Ensures every response carries X-Correlation-Id (Guid).
/// </summary>
public sealed class CorrelationHeaderMiddleware
{
    private readonly RequestDelegate _next;
    public CorrelationHeaderMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext context, ICorrelationIdProvider corr)
    {
        var cid = corr.GetCorrelationId().ToString();
        context.Response.OnStarting(() =>
        {
            context.Response.Headers["X-Correlation-Id"] = cid;
            return Task.CompletedTask;
        });
        await _next(context);
    }
}