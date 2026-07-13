using GMMS.App.Services;
using GMMS.Domain;
using GMMS.Domain.Features.MemberShip.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GMMS.App.Feature.Membership
{
    public partial class MembershipDelete : ComponentBase
    {
        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; } = null!;

        [Parameter]
        public int MembershipId { get; set; }

        [Parameter]
        public int MemberId { get; set; }

        [Inject]
        private ApiService ApiService { get; set; } = null!;

        private MemberShipModel? membership;
        private bool isLoading = true;
        private bool isDeleting;
        private string? errorMessage;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var result = await ApiService.GetMembershipDetailsAsync<Result<MembershipDetailModel>>(MembershipId);
                if (result?.IsSuccess == true && result.Data is not null)
                {
                    membership = new MemberShipModel
                    {
                        MembershipId = result.Data.MembershipId,
                        MemberCode = result.Data.MemberCode,
                        MemberName = result.Data.MemberName,
                        PlanCode = result.Data.PlanCode,
                        PlanName = result.Data.PlanName,
                        StartDate = result.Data.StartDate,
                        EndDate = result.Data.EndDate,
                        Status = result.Data.Status
                    };
                    if (MemberId <= 0)
                        MemberId = result.Data.MemberId;
                }
                else
                {
                    errorMessage = result?.Message ?? "Failed to load membership.";
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
                var result = await ApiService.DeleteMembershipAsync<Result<bool>>(MembershipId);
                if (result?.IsSuccess == true)
                {
                    MudDialog.Close(DialogResult.Ok(true));
                }
                else
                {
                    errorMessage = result?.Message ?? "Failed to delete membership.";
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
