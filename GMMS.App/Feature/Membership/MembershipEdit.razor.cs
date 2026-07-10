using GMMS.App.Services;
using GMMS.Domain;
using GMMS.Domain.Features.MemberShip.Models;
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

        private UpdateMembershipRequestModel request = new();
        private bool isLoading;
        private string? errorMessage;

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;

            try
            {
                var result = await ApiService.GetMembershipDetailsAsync<Result<MembershipDetailModel>>(Id);
                if (result?.IsSuccess == true && result.Data is not null)
                {
                    request.MembershipId = result.Data.MembershipId;
                    request.MembershipPlanId = result.Data.MembershipPlanId;
                }
                else
                {
                    errorMessage = result?.Message ?? "Membership not found.";
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

        private async Task Update()
        {
            errorMessage = null;

            try
            {
                var result = await ApiService.UpdateMembershipAsync<UpdateMembershipRequestModel, Result<MemberShipModel>>(Id, request);
                if (result?.IsSuccess == true)
                {
                    Navigation.NavigateTo("/membership-list");
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
        }
    }
}
