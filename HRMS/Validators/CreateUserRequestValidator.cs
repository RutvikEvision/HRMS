using FluentValidation;
using HRMS.DTOs.UsersDTOs;

namespace HRMS.Validators
{
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(256);

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+?[0-9\s\-]{7,15}$").WithMessage("Enter a valid phone number.")
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

            RuleFor(x => x.Address)
                .MaximumLength(300);

            RuleFor(x => x.Password)
                .MinimumLength(8)
                .When(x => !string.IsNullOrWhiteSpace(x.Password));
        }
    }
}
