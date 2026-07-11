using GMMS.App.Services;
using GMMS.Domain;
using GMMS.Domain.Features.MemberShip.Models;
using GMMS.Domain.Features.MemberShipPlan.Models;
using Microsoft.AspNetCore.Components;

namespace GMMS.App.Feature.Membership
{
    public partial class MembershipEdit : ComponentBase
    {
        [Parameter]
        public int Id { get; set; }

        [Inject]
        private ApiService ApiService { get; set; } = null!;

        [Inject]
        private NavigationManager Navigation { get; set; } = null!;

        [SupplyParameterFromQuery(Name = "memberId")]
        public int MemberId { get; set; }

        private UpdateMembershipRequestModel request = new();
        private MembershipDetailModel? detail;
        private List<MemberShipPlanModel>? plans;
        private bool isLoadingData = true;
        private bool isSaving;
        private string? errorMessage;

        private DateOnly? newEndDate;

        private string _planIdStr
        {
            get => __planIdStr;
            set
            {
                __planIdStr = value;
                RecalcEndDate();
            }
        }
        private string __planIdStr = "";

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var result = await ApiService.GetMembershipDetailsAsync<Result<MembershipDetailModel>>(Id);
                if (result?.IsSuccess == true && result.Data is not null)
                {
                    detail = result.Data;
                    request.MembershipId = result.Data.MembershipId;
                    request.MembershipPlanId = result.Data.MembershipPlanId;
                    _planIdStr = result.Data.MembershipPlanId.ToString();
                    if (MemberId <= 0)
                        MemberId = result.Data.MemberId;
                }
                else
                {
                    errorMessage = result?.Message ?? "Membership not found.";
                }

                var planResult = await ApiService.GetMembershipPlanListAsync<Result<MemberShipPlanListResponseModel>>(1, 1000);
                if (planResult?.IsSuccess == true && planResult.Data is not null)
                    plans = planResult.Data.MemberShipPlans;
                
                if (detail is not null && plans is not null)
                    RecalcEndDate();
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

        private void OnInvalidSubmit()
        {
            if (request.MembershipPlanId <= 0)
                errorMessage = "Please select a membership plan.";
        }

        private void RecalcEndDate()
        {
            int.TryParse(_planIdStr, out var planId);
            if (detail is null || plans is null || planId <= 0)
            {
                newEndDate = null;
                return;
            }
            var plan = plans.FirstOrDefault(p => p.MemberShipPlanId == planId);
            newEndDate = plan is not null
                ? detail.StartDate.AddDays(plan.DurationDays)
                : null;
        }

        private async Task Update()
        {
            RecalcEndDate();
            int.TryParse(_planIdStr, out var planId);
            request.MembershipPlanId = planId;

            if (request.MembershipPlanId <= 0)
            {
                errorMessage = "Please select a membership plan.";
                return;
            }

            isSaving = true;
            errorMessage = null;

            try
            {
                var result = await ApiService.UpdateMembershipAsync<UpdateMembershipRequestModel, Result<MembershipDetailModel>>(Id, request);
                if (result?.IsSuccess == true)
                {
                    Navigation.NavigateTo($"/membership-list?memberId={MemberId}");
                }
                else
                {
                    errorMessage = result?.Message ?? "Failed to update membership.";
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
