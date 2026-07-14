using GMMS.App.Services;
using GMMS.Domain;
using GMMS.Domain.Features.MemberShip.Models;
using GMMS.Domain.Features.MemberShipPlan.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GMMS.App.Feature.Membership
{
    public partial class MembershipEdit : ComponentBase
    {
        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; } = null!;

        [Parameter]
        public int MembershipId { get; set; }

        [Parameter]
        public int MemberId { get; set; }

        [Inject]
        private ApiService ApiService { get; set; } = null!;

        private UpdateMembershipRequestModel request = new();
        private MembershipDetailModel? detail;
        private List<MemberShipPlanModel> plans = new();
        private bool isLoadingData = true;
        private bool isSaving;
        private string? errorMessage;

        private DateOnly? newEndDate;

        private string _planStr
        {
            get => request.MembershipPlanId > 0 ? request.MembershipPlanId.ToString() : "";
            set
            {
                request.MembershipPlanId = int.TryParse(value, out var id) ? id : 0;
                RecalcEndDate();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var result = await ApiService.GetMembershipDetailsAsync<Result<MembershipDetailModel>>(MembershipId);
                if (result?.IsSuccess == true && result.Data is not null)
                {
                    detail = result.Data;
                    request.MembershipId = result.Data.MembershipId;
                    request.MembershipPlanId = result.Data.MembershipPlanId;
                    if (MemberId <= 0)
                        MemberId = result.Data.MemberId;
                }
                else
                {
                    errorMessage = result?.Message ?? "Membership not found.";
                }

                var planResult = await ApiService.GetMembershipPlanListAsync<Result<MemberShipPlanListResponseModel>>(1, 100);
                if (planResult?.IsSuccess == true && planResult.Data is not null)
                    plans = planResult.Data.MemberShipPlans ?? new();
                else if (string.IsNullOrEmpty(errorMessage))
                    errorMessage = planResult?.Message ?? "Failed to load membership plans.";

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
                StateHasChanged();
            }
        }

        private void RecalcEndDate()
        {
            if (detail is null || plans is null || request.MembershipPlanId <= 0)
            {
                newEndDate = null;
                return;
            }
            var plan = plans.FirstOrDefault(p => p.MemberShipPlanId == request.MembershipPlanId);
            newEndDate = plan is not null
                ? detail.StartDate.AddDays(plan.DurationDays)
                : null;
        }

        private void Cancel()
        {
            MudDialog.Cancel();
        }

        private async Task Update()
        {
            if (request.MembershipPlanId <= 0)
            {
                errorMessage = "Please select a membership plan.";
                return;
            }

            isSaving = true;
            errorMessage = null;

            try
            {
                var result = await ApiService.UpdateMembershipAsync<UpdateMembershipRequestModel, Result<MembershipDetailModel>>(MembershipId, request);
                if (result?.IsSuccess == true)
                {
                    MudDialog.Close(DialogResult.Ok(true));
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