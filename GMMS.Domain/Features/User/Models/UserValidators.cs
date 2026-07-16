using FluentValidation;

namespace GMMS.Domain.Features.User.Models;

public class UserListRequestValidator : AbstractValidator<UserListRequestModel>
{
    public UserListRequestValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("Page number must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");
    }
}

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequestModel>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Username is required.")
            .Length(3, 100).WithMessage("Username must be between 3 and 100 characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$")
            .WithMessage("Password must contain at least one uppercase, one lowercase, one number, and one special character.");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required.")
            .Must(r => r == "Owner" || r == "Admin").WithMessage("Role must be 'Owner' or 'Admin'.");
    }
}

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequestModel>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("User ID is required.");

        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Username is required.")
            .Length(3, 100).WithMessage("Username must be between 3 and 100 characters.");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required.")
            .Must(r => r == "Owner" || r == "Admin").WithMessage("Role must be 'Owner' or 'Admin'.");
    }
}

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequestModel>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("User ID is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$")
            .WithMessage("Password must contain at least one uppercase, one lowercase, one number, and one special character.");
    }
}
