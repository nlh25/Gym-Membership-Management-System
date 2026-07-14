using FluentValidation;
using GMMS.Domain.Features.Member.Models;

namespace GMMS.Domain.Features.Member.Models
{
    public class CreateMemberRequestValidator : AbstractValidator<CreateMemberRequestModel>
    {
        public CreateMemberRequestValidator()
        {
            RuleFor(x => x.MemberCode)
                .NotEmpty().WithMessage("Member code is required.")
                .MaximumLength(50).WithMessage("Member code must not exceed 50 characters.")
                .Matches("^[A-Z0-9-]+$").WithMessage("Member code can only contain uppercase letters, numbers, and hyphens.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Member name is required.")
                .MaximumLength(100).WithMessage("Member name must not exceed 100 characters.")
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Member name cannot be only whitespace.");
        }
    }

    public class UpdateMemberRequestValidator : AbstractValidator<UpdateMemberRequestModel>
    {
        public UpdateMemberRequestValidator()
        {
            RuleFor(x => x.MemberId)
                .GreaterThan(0).WithMessage("Member ID must be greater than 0.");

            RuleFor(x => x.MemberCode)
                .NotEmpty().WithMessage("Member code is required.")
                .MaximumLength(50).WithMessage("Member code must not exceed 50 characters.")
                .Matches("^[A-Z0-9-]+$").WithMessage("Member code can only contain uppercase letters, numbers, and hyphens.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Member name is required.")
                .MaximumLength(100).WithMessage("Member name must not exceed 100 characters.")
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Member name cannot be only whitespace.");
        }
    }

    public class MemberListRequestValidator : AbstractValidator<MemberListRequestModel>
    {
        public MemberListRequestValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1).WithMessage("Page number must be at least 1.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");

            RuleFor(x => x.SearchTerm)
                .MaximumLength(100).WithMessage("Search term must not exceed 100 characters.")
                .When(x => !string.IsNullOrEmpty(x.SearchTerm));
        }
    }
}