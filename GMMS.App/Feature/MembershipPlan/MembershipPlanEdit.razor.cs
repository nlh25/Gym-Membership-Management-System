using GMMS.App.Services;
using GMMS.Domain;
using GMMS.Domain.Features.MemberShipPlan.Models;
using Microsoft.AspNetCore.Components;

namespace GMMS.App.Feature.MembershipPlan
{
    public partial class MembershipPlanEdit : ComponentBase
    {
        [Parameter]
        public int Id { get; set; }

        [Inject]
        private ApiService ApiService { get; set; } = null!;

        [Inject]
        private NavigationManager Navigation { get; set; } = null!;

        private UpdateMemberShipPlanRequestModel request = new();
        private bool isLoading;
        private string? errorMessage;

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;

            try
            {
                var result = await ApiService.GetMembershipPlanDetailsAsync<Result<MembershipPlanDetailModel>>(Id);
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

        private async Task Update()
        {
            errorMessage = null;

            try
            {
                var result = await ApiService.UpdateMembershipPlanAsync<UpdateMemberShipPlanRequestModel, Result<MemberShipPlanModel>>(Id, request);
                if (result?.IsSuccess == true)
                {
                    Navigation.NavigateTo("/membershipplan-list");
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
        }
    }
}
