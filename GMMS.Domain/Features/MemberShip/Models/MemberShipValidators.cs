using FluentValidation;
using GMMS.Domain.Enums;
using GMMS.Domain.Features.MemberShip.Models;

namespace GMMS.Domain.Features.MemberShip.Models
{
    public class MemberShipListRequestValidator : AbstractValidator<MemberShipListRequestModel>
    {
        public MemberShipListRequestValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1).WithMessage("Page number must be at least 1.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");

            RuleFor(x => x.MemberId)
                .GreaterThan(0).WithMessage("Member ID is required and must be greater than 0.");
        }
    }

    public class AllMemberShipListRequestValidator : AbstractValidator<AllMemberShipListRequestModel>
    {
        public AllMemberShipListRequestValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1).WithMessage("Page number must be at least 1.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");

            RuleFor(x => x.SearchTerm)
                .MaximumLength(100).WithMessage("Search term must not exceed 100 characters.")
                .When(x => !string.IsNullOrEmpty(x.SearchTerm));

            RuleFor(x => x.Status)
                .Must(BeValidStatus).WithMessage("Status must be one of: Pending, Active, Expired, Cancelled.")
                .When(x => !string.IsNullOrEmpty(x.Status));
        }

        private bool BeValidStatus(string? status)
        {
            if (string.IsNullOrEmpty(status)) return true;
            return Enum.TryParse<MembershipPlanStatus>(status, true, out _);
        }
    }

    public class CreateMemberShipRequestValidator : AbstractValidator<CreateMemberShipRequestModel>
    {
        public CreateMemberShipRequestValidator()
        {
            RuleFor(x => x.MemberId)
                .GreaterThan(0).WithMessage("Member ID is required.");

            RuleFor(x => x.MembershipPlanId)
                .GreaterThan(0).WithMessage("Membership Plan ID is required.");

            RuleFor(x => x.PaymentMethodId)
                .GreaterThan(0).WithMessage("Payment Method ID is required.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than 0.")
                .LessThanOrEqualTo(1000000).WithMessage("Amount cannot exceed 1,000,000.");

            RuleFor(x => x.Sspath)
                .MaximumLength(500).WithMessage("Receipt path must not exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.Sspath));
        }
    }

    public class UpdateMembershipRequestValidator : AbstractValidator<UpdateMembershipRequestModel>
    {
        public UpdateMembershipRequestValidator()
        {
            RuleFor(x => x.MembershipId)
                .GreaterThan(0).WithMessage("Membership ID is required.");

            RuleFor(x => x.MembershipPlanId)
                .GreaterThan(0).WithMessage("Membership Plan ID is required.");
        }
    }
}