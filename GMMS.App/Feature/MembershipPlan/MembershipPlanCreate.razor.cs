using GMMS.App.Services;
using GMMS.Domain;
using GMMS.Domain.Features.MemberShipPlan.Models;
using Microsoft.AspNetCore.Components;

namespace GMMS.App.Feature.MembershipPlan
{
    public partial class MembershipPlanCreate : ComponentBase
    {
        [Inject]
        private ApiService ApiService { get; set; } = null!;

        [Inject]
        private NavigationManager Navigation { get; set; } = null!;

        private CreateMemberShipPlanRequestModel request = new();
        private bool isLoading;
        private string? errorMessage;

        private async Task Save()
        {
            isLoading = true;
            errorMessage = null;

            try
            {
                var result = await ApiService.CreateMembershipPlanAsync<CreateMemberShipPlanRequestModel, Result<MemberShipPlanModel>>(request);

                if (result?.IsSuccess == true)
                {
                    Navigation.NavigateTo("/membershipplan-list");
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
                isLoading = false;
            }
        }
    }
}
