using ComplianceCrm.Application.Documents.Dtos;
using FluentValidation;

namespace ComplianceCrm.Application.Documents.Validators;

public sealed class UpdateDocumentMetadataRequestValidator : AbstractValidator<UpdateDocumentMetadataRequest>
{
    public UpdateDocumentMetadataRequestValidator()
    {
        When(x => x.OriginalFileName != null, () =>
        {
            RuleFor(x => x.OriginalFileName!)
                .NotEmpty()
                .MaximumLength(255);
        });

        When(x => x.ContentType != null, () =>
        {
            RuleFor(x => x.ContentType!)
                .NotEmpty()
                .MaximumLength(128);
        });

        When(x => x.RetentionUntilUtc.HasValue, () =>
        {
            RuleFor(x => x.RetentionUntilUtc!.Value)
                .GreaterThan(DateTime.UtcNow.AddMinutes(-1))
                .WithMessage("RetentionUntilUtc must be in the future.");
        });

        When(x => x.Type.HasValue, () =>
        {
            RuleFor(x => x.Type!.Value)
                .Must(v => v is 0 or 1 or 2 or 3 or 4 or 5 or 6 or 7 or 99)
                .WithMessage("Invalid DocumentType.");
        });

        When(x => x.Sensitivity.HasValue, () =>
        {
            RuleFor(x => x.Sensitivity!.Value)
                .Must(v => v is 0 or 1 or 2)
                .WithMessage("Invalid DocumentSensitivity.");
        });

        When(x => x.Status.HasValue, () =>
        {
            RuleFor(x => x.Status!.Value)
                .Must(v => v is 1 or 2 or 3)
                .WithMessage("Invalid DocumentStatus.");
        });
    }
}
