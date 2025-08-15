using FluentValidation;
using ComplianceCrm.Application.Customers.Dtos;

namespace ComplianceCrm.Application.Customers.Validators;

/// <summary>
/// Validates input for creating a new Customer.
/// </summary>
public sealed class CreateCustomerRequestValidator : AbstractValidator<CreateCustomerRequest>
{
    public CreateCustomerRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email format is invalid.")
            .MaximumLength(200).WithMessage("Email cannot exceed 200 characters.");

        RuleFor(x => x.Phone)
            .MaximumLength(30).WithMessage("Phone cannot exceed 30 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));

        // Business rule hint (optional):
        // If you later add 'RequiresConsent' per tenant, validate Consented accordingly here.
    }
}
