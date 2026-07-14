using GMMS.App.Services;
using GMMS.Domain;
using GMMS.Domain.Features.MemberShipPlan.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GMMS.App.Feature.MembershipPlan
{
    public partial class MembershipPlanEdit : ComponentBase
    {
        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; } = null!;

        [Parameter]
        public int PlanId { get; set; }

        [Inject]
        private ApiService ApiService { get; set; } = null!;

        private UpdateMemberShipPlanRequestModel request = new();
        private bool isLoading = true;
        private bool isSaving;
        private string? errorMessage;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var result = await ApiService.GetMembershipPlanDetailsAsync<Result<MembershipPlanDetailModel>>(PlanId);
                if (result?.IsSuccess == true && result.Data is not null)
                {
                    request.MemberShipPlanId = result.Data.MemberShipPlanId;
                    request.PlanCode = result.Data.PlanCode;
                    request.PlanName = result.Data.PlanName;
                    request.Price = result.Data.Price;
                    request.DurationDays = result.Data.DurationDays;
                    request.Description = result.Data.Description;
                }
                else
                {
                    errorMessage = result?.Message ?? "Membership plan not found.";
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            finally
            {
                isLoading = false;
            }
        }

        private void Cancel()
        {
            MudDialog.Cancel();
        }

        private async Task Update()
        {
            isSaving = true;
            errorMessage = null;

            try
            {
                var result = await ApiService.UpdateMembershipPlanAsync<UpdateMemberShipPlanRequestModel, Result<MemberShipPlanModel>>(PlanId, request);
                if (result?.IsSuccess == true)
                {
                    MudDialog.Close(DialogResult.Ok(true));
                }
                else
                {
                    errorMessage = result?.Message ?? "Failed to update membership plan.";
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