using GMMS.App.Services;
using GMMS.Domain;
using GMMS.Domain.Features.MemberShip.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace GMMS.App.Feature.Membership
{
    public partial class MembershipDelete : ComponentBase
    {
        [Parameter]
        public int Id { get; set; }

        [Inject]
        private ApiService ApiService { get; set; } = null!;

        [Inject]
        private NavigationManager Nav { get; set; } = null!;

        private MemberShipModel? membership;
        private bool isLoading = true;
        private bool isDeleting;
        private string? errorMessage;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var result = await ApiService.GetMembershipDetailsAsync<Result<MembershipDetailModel>>(Id);
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

        private async Task ConfirmDelete()
        {
            isDeleting = true;
            errorMessage = null;
            StateHasChanged();

            try
            {
                var result = await ApiService.DeleteMembershipAsync<Result<bool>>(Id);
                if (result?.IsSuccess == true)
                {
                    Nav.NavigateTo("membership-list");
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
