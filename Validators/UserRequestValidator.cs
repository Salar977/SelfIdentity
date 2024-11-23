using FluentValidation;
using SelfIdentity.DTOs;

namespace SelfIdentity.Validators;

public class UserRequestValidator : AbstractValidator<UserRequest>
{
    public UserRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username cannot be empty!")
            .MinimumLength(5).WithMessage("Username must be atleast 5 characters!")
            .MaximumLength(50).WithMessage("Username must be max 50 characters!");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email cannot be empty!")
            .EmailAddress().WithMessage("Email must be a valid email address");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password must be provided")
            .MinimumLength(5).WithMessage("Password must be at least 5 characters long")
            .MaximumLength(50).WithMessage("Password cannot be longer than 50 characters")
            .Matches(@"^[a-zA-Z0-9æøåÆØÅ\-_]+$").WithMessage("Password can only contain letters, numbers, hyphens, and underscores");
    }
}
