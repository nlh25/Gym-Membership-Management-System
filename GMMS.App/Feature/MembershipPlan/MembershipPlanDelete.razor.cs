using GMMS.App.Services;
using GMMS.Domain;
using GMMS.Domain.Features.MemberShipPlan.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GMMS.App.Feature.MembershipPlan
{
    public partial class MembershipPlanDelete : ComponentBase
    {
        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; } = null!;

        [Parameter]
        public int PlanId { get; set; }

        [Inject]
        private ApiService ApiService { get; set; } = null!;

        [Inject]
        private ISnackbar Snackbar { get; set; } = null!;

        private MemberShipPlanModel? plan;
        private bool isLoading = true;
        private bool isDeleting;
        private string? errorMessage;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var result = await ApiService.GetMembershipPlanDetailsAsync<Result<MembershipPlanDetailModel>>(PlanId);
                if (result?.IsSuccess == true && result.Data is not null)
                {
                    plan = new MemberShipPlanModel
                    {
                        MemberShipPlanId = result.Data.MemberShipPlanId,
                        PlanCode = result.Data.PlanCode,
                        PlanName = result.Data.PlanName,
                        Price = result.Data.Price,
                        DurationDays = result.Data.DurationDays,
                        Description = result.Data.Description,
                        IsActive = true, // default for display
                        CreatedAt = result.Data.CreatedAt,
                        UpdatedAt = result.Data.UpdatedAt
                    };
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

        private void Cancel()
        {
            MudDialog.Cancel();
        }

        private async Task ConfirmDelete()
        {
            isDeleting = true;
            errorMessage = null;
            StateHasChanged();

            try
            {
                var result = await ApiService.DeleteMembershipPlanAsync<Result<bool>>(PlanId);
                if (result?.IsSuccess == true)
                {
                    Snackbar.Add("Membership plan deleted successfully!", Severity.Success);
                    MudDialog.Close(DialogResult.Ok(true));
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