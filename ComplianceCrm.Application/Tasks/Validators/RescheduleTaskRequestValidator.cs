using ComplianceCrm.Application.Tasks.Dtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplianceCrm.Application.Tasks.Validators;

/// <summary>Validates reschedule payload to ensure UTC and consistency.</summary>
public sealed class RescheduleTaskRequestValidator : AbstractValidator<RescheduleTaskRequest>
{
    public RescheduleTaskRequestValidator()
    {
        RuleFor(x => x.StartDateUtc)
            .NotEmpty().WithMessage("StartDateUtc is required")
            .Must(d => d.Kind == DateTimeKind.Utc)
            .WithMessage("StartDateUtc must be in UTC");

        When(x => x.EndDateUtc.HasValue, () =>
        {
            RuleFor(x => x.EndDateUtc!.Value)
                .Must(d => d.Kind == DateTimeKind.Utc)
                .WithMessage("EndDateUtc must be in UTC");

            RuleFor(x => x)
                .Must(x => x.EndDateUtc!.Value > x.StartDateUtc)
                .WithMessage("EndDateUtc must be after StartDateUtc");
        });
    }
}