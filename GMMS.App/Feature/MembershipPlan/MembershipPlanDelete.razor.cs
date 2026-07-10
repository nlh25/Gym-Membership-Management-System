using GMMS.App.Services;
using GMMS.Domain;
using GMMS.Domain.Features.MemberShipPlan.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace GMMS.App.Feature.MembershipPlan
{
    public partial class MembershipPlanDelete : ComponentBase
    {
        [Parameter]
        public int Id { get; set; }

        [Inject]
        private ApiService ApiService { get; set; } = null!;

        [Inject]
        private NavigationManager Nav { get; set; } = null!;

        private MembershipPlanDetailModel? plan;
        private bool isLoading = true;
        private bool isDeleting;
        private string? errorMessage;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var result = await ApiService.GetMembershipPlanDetailsAsync<Result<MembershipPlanDetailModel>>(Id);
                if (result?.IsSuccess == true && result.Data is not null)
                {
                    plan = result.Data;
                }
                else
                {
                    errorMessage = result?.Message ?? "Failed to load membership plan.";
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

        private async Task ConfirmDelete()
        {
            isDeleting = true;
            errorMessage = null;
            StateHasChanged();

            try
            {
                var result = await ApiService.DeleteMembershipPlanAsync<Result<bool>>(Id);
                if (result?.IsSuccess == true)
                {
                    Nav.NavigateTo("membershipplan-list");
                }
                else
                {
                    errorMessage = result?.Message ?? "Failed to delete membership plan.";
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            finally
            {
                isDeleting = false;
            }
        }
    }
}
