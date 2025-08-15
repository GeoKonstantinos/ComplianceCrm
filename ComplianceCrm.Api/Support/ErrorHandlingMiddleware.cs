using ComplianceCrm.Application.Abstractions.Providers;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using FluentValidation;

namespace ComplianceCrm.Api.Support;

/// <summary>
/// Converts exceptions into RFC7807 ProblemDetails with trace & correlation ids.
/// </summary>
public sealed class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    // ⬇️ Method injection: παίρνουμε τον ICorrelationIdProvider ανά request
    public async Task Invoke(HttpContext context, ComplianceCrm.Application.Abstractions.Providers.ICorrelationIdProvider corr)
    {
        try
        {
            await _next(context);
        }
        catch (FluentValidation.ValidationException vex)
        {
            var pd = new ValidationProblemDetails(
                vex.Errors
                   .GroupBy(e => e.PropertyName)
                   .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()))
            {
                Title = "Validation failed",
                Status = StatusCodes.Status400BadRequest,
                Instance = context.Request.Path
            };
            AttachMeta(pd, context, corr);
            await WriteProblem(context, pd);
        }
        catch (KeyNotFoundException kex)
        {
            var pd = new ProblemDetails
            {
                Title = "Not found",
                Detail = kex.Message,
                Status = StatusCodes.Status404NotFound,
                Instance = context.Request.Path
            };
            AttachMeta(pd, context, corr);
            await WriteProblem(context, pd);
        }
        catch (InvalidOperationException ioex)
        {
            var pd = new ProblemDetails
            {
                Title = "Invalid operation",
                Detail = ioex.Message,
                Status = StatusCodes.Status409Conflict,
                Instance = context.Request.Path
            };
            AttachMeta(pd, context, corr);
            await WriteProblem(context, pd);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            var pd = new ProblemDetails
            {
                Title = "Internal server error",
                Status = StatusCodes.Status500InternalServerError,
                Instance = context.Request.Path
            };
            AttachMeta(pd, context, corr);
            await WriteProblem(context, pd);
        }
    }

    private static void AttachMeta(ProblemDetails pd, HttpContext ctx, ComplianceCrm.Application.Abstractions.Providers.ICorrelationIdProvider corr)
    {
        pd.Extensions["traceId"] = ctx.TraceIdentifier;
        pd.Extensions["correlationId"] = corr.GetCorrelationId().ToString();
    }

    private static async Task WriteProblem(HttpContext ctx, ProblemDetails pd)
    {
        ctx.Response.ContentType = "application/problem+json";
        ctx.Response.StatusCode = pd.Status ?? StatusCodes.Status500InternalServerError;
        await ctx.Response.WriteAsync(JsonSerializer.Serialize(pd));
    }
}