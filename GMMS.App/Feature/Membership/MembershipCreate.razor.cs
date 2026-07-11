using GMMS.App.Services;
using GMMS.Domain;
using GMMS.Domain.Features.Member.Models;
using GMMS.Domain.Features.MemberShip.Models;
using GMMS.Domain.Features.MemberShipPlan.Models;
using GMMS.Domain.Features.PaymentMethod.Models;
using Microsoft.AspNetCore.Components;

namespace GMMS.App.Feature.Membership
{
    public partial class MembershipCreate : ComponentBase
    {
        [Inject]
        private ApiService ApiService { get; set; } = null!;

        [Inject]
        private NavigationManager Navigation { get; set; } = null!;

        [SupplyParameterFromQuery(Name = "memberId")]
        public int MemberId { get; set; }

        private CreateMemberShipRequestModel request = new();
        private List<MemberModel>? members;
        private List<MemberShipPlanModel>? plans;
        private List<PaymentMethodModel>? paymentMethods;
        private bool isLoadingData = true;
        private bool isSaving;
        private string? errorMessage;

        private string _memberIdStr { get; set; } = "";
        private string _planIdStr { get; set; } = "";
        private string _paymentMethodIdStr { get; set; } = "";

        protected override async Task OnInitializedAsync()
        {
            try
            {
                if (MemberId > 0)
                {
                    request.MemberId = MemberId;
                    _memberIdStr = MemberId.ToString();
                }

                var memberResult = await ApiService.GetMemberListAsync<Result<MemberListResponseModel>>(1, 1000);
                if (memberResult?.IsSuccess == true && memberResult.Data is not null)
                    members = memberResult.Data.Members;

                var planResult = await ApiService.GetMembershipPlanListAsync<Result<MemberShipPlanListResponseModel>>(1, 1000);
                if (planResult?.IsSuccess == true && planResult.Data is not null)
                    plans = planResult.Data.MemberShipPlans;

                var methodResult = await ApiService.GetPaymentMethodListAsync<Result<PaymentMethodListResponseModel>>(1, 1000);
                if (methodResult?.IsSuccess == true && methodResult.Data is not null)
                    paymentMethods = methodResult.Data.PaymentMethods;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            finally
            {
                isLoadingData = false;
            }
        }

        private async Task Save()
        {
            int.TryParse(_memberIdStr, out var memberId);
            int.TryParse(_planIdStr, out var planId);
            int.TryParse(_paymentMethodIdStr, out var paymentMethodId);

            request.MemberId = memberId;
            request.MembershipPlanId = planId;
            request.PaymentMethodId = paymentMethodId;

            if (request.MemberId <= 0)
            {
                errorMessage = "Please select a member.";
                return;
            }
            if (request.MembershipPlanId <= 0)
            {
                errorMessage = "Please select a membership plan.";
                return;
            }
            if (request.PaymentMethodId <= 0)
            {
                errorMessage = "Please select a payment method.";
                return;
            }

            isSaving = true;
            errorMessage = null;

            try
            {
                var result = await ApiService.CreateMembershipAsync<CreateMemberShipRequestModel, Result<MembershipDetailModel>>(request);

                if (result?.IsSuccess == true)
                {
                    Navigation.NavigateTo($"/membership-list?memberId={request.MemberId}");
                }
                else
                {
                    errorMessage = result?.Message ?? "Failed to create membership.";
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            finally
            {
                isSaving = false;
            }
        }
    }
}
