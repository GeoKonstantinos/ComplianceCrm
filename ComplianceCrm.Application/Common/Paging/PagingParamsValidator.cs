using FluentValidation;

namespace ComplianceCrm.Application.Common.Paging;

/// <summary>
/// Validates standard paging parameters.
/// </summary>
public sealed class PagingParamsValidator : AbstractValidator<PagingParams>
{
    public const int MaxPageSize = 200;

    public PagingParamsValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be >= 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, MaxPageSize)
            .WithMessage($"PageSize must be between 1 and {MaxPageSize}.");
    }
}
