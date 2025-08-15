using ComplianceCrm.Application.Tasks.Dtos;
using FluentValidation;

namespace ComplianceCrm.Application.Tasks.Validators;

public sealed class UpdateTaskStatusRequestValidator : AbstractValidator<UpdateTaskStatusRequest>
{
    public UpdateTaskStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .Must(v => v == 1 || v == 2 || v == 3 || v == 4) // Pending/InProgress/Done/Overdue
            .WithMessage("Invalid TaskStatus.");

        When(x => x.Status == 3, () => // Done
        {
            RuleFor(x => x.CompletedAtUtc)
                .NotNull().WithMessage("CompletedAtUtc is required when setting status to Done.")
                .LessThanOrEqualTo(DateTime.UtcNow.AddMinutes(1))
                .WithMessage("CompletedAtUtc cannot be in the distant future.");
        });
    }
}
