using ComplianceCrm.Application.Customers.Dtos;
using FluentValidation;

namespace ComplianceCrm.Application.Customers.Validators;

public class CustomerValidator : AbstractValidator<CustomerDto>
{
    public CustomerValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters");

        RuleFor(c => c.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(c => c.Phone)
            .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters");
    }
}