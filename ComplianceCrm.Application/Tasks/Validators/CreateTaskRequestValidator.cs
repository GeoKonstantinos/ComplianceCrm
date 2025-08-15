using ComplianceCrm.Application.Tasks.Dtos;
using FluentValidation;

namespace ComplianceCrm.Application.Tasks.Validators;

public sealed class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskRequestValidator()
    {
        RuleFor(x => x.CustomerId)
            .GreaterThan(0).WithMessage("CustomerId must be > 0.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.Type)
            .Must(v => v == 1 || v == 2 || v == 3 || v == 99)
            .WithMessage("Invalid TaskType.");

        RuleFor(x => x.DueDateUtc)
            .GreaterThan(DateTime.UtcNow.AddMinutes(-1))
            .WithMessage("DueDateUtc must be in the future.");
    }
}
