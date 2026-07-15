using GMMS.App.Services;
using GMMS.Domain;
using GMMS.Domain.Features.MemberShipPlan.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GMMS.App.Feature.MembershipPlan
{
    public partial class MembershipPlanDetail : ComponentBase
    {
        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; } = null!;

        [Parameter]
        public int PlanId { get; set; }

        [Inject]
        private ApiService ApiService { get; set; } = null!;

        private MembershipPlanDetailModel? plan;
        private bool isLoading = true;
        private string? errorMessage;

        protected override async Task OnInitializedAsync()
        {
            await LoadPlanDetails();
        }

        private async Task LoadPlanDetails()
        {
            try
            {
                var result = await ApiService.GetMembershipPlanDetailsAsync<Result<MembershipPlanDetailModel>>(PlanId);
                if (result?.IsSuccess == true && result.Data is not null)
                {
                    plan = result.Data;
                }
                else
                {
                    errorMessage = result?.Message ?? "Failed to load plan details.";
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
    }
}