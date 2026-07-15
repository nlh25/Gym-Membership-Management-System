using GMMS.App.Services;
using GMMS.Domain;
using GMMS.Domain.Features.MemberShipPlan.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GMMS.App.Feature.MembershipPlan
{
    public partial class MembershipPlanCreate : ComponentBase
    {
        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; } = null!;

        [Inject]
        private ApiService ApiService { get; set; } = null!;

        [Inject]
        private ISnackbar Snackbar { get; set; } = null!;

        private CreateMemberShipPlanRequestModel request = new();
        private bool isSaving;
        private string? errorMessage;

        private void Cancel()
        {
            MudDialog.Cancel();
        }

        private async Task Save()
        {
            isSaving = true;
            errorMessage = null;

            try
            {
                var result = await ApiService.CreateMembershipPlanAsync<CreateMemberShipPlanRequestModel, Result<MemberShipPlanModel>>(request);

                if (result?.IsSuccess == true)
                {
                    Snackbar.Add("Membership plan created successfully!", Severity.Success);
                    MudDialog.Close(DialogResult.Ok(true));
                }
                else
                {
                    errorMessage = result?.Message ?? "Failed to create membership plan.";
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