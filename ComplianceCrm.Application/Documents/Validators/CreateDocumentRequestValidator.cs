using ComplianceCrm.Application.Documents.Dtos;
using FluentValidation;

namespace ComplianceCrm.Application.Documents.Validators;

public sealed class CreateDocumentRequestValidator : AbstractValidator<CreateDocumentRequest>
{
    public CreateDocumentRequestValidator()
    {
        RuleFor(x => x.CustomerId)
            .GreaterThan(0).WithMessage("CustomerId must be > 0.");

        RuleFor(x => x.Type)
            .Must(v => v is 0 or 1 or 2 or 3 or 4 or 5 or 6 or 7 or 99)
            .WithMessage("Invalid DocumentType.");

        RuleFor(x => x.Sensitivity)
            .Must(v => v is 0 or 1 or 2)
            .WithMessage("Invalid DocumentSensitivity.");

        RuleFor(x => x.OriginalFileName)
            .NotEmpty().WithMessage("OriginalFileName is required.")
            .MaximumLength(255);

        RuleFor(x => x.ContentType)
            .NotEmpty().WithMessage("ContentType is required.")
            .MaximumLength(128);

        RuleFor(x => x.SizeBytes)
            .GreaterThan(0).WithMessage("SizeBytes must be > 0.");

        RuleFor(x => x.Sha256)
            .NotEmpty().WithMessage("Sha256 is required.")
            .Length(64).WithMessage("Sha256 must be 64 hex characters.")
            .Matches("^[a-fA-F0-9]{64}$").WithMessage("Sha256 must be a valid hex string.");

        RuleFor(x => x.Provider)
            .Must(v => v is 1 or 2 or 3 or 4)
            .WithMessage("Invalid StorageProvider.");

        RuleFor(x => x.StoragePath)
            .NotEmpty().WithMessage("StoragePath is required.")
            .MaximumLength(500);

        When(x => x.RetentionUntilUtc.HasValue, () =>
        {
            RuleFor(x => x.RetentionUntilUtc!.Value)
                .GreaterThan(DateTime.UtcNow.AddMinutes(-1))
                .WithMessage("RetentionUntilUtc must be in the future.");
        });
    }
}
