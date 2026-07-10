using GMMS.App.Services;
using GMMS.Domain;
using GMMS.Domain.Features.MemberShipPlan.Models;
using Microsoft.AspNetCore.Components;

namespace GMMS.App.Feature.MembershipPlan
{
    public partial class MembershipPlanList : ComponentBase
    {
        [Inject]
        private ApiService ApiService { get; set; } = null!;

        [SupplyParameterFromQuery(Name = "page")]
        public int Page { get; set; } = 1;

        private List<MemberShipPlanModel>? plans;
        private int pageNumber = 1;
        private int pageSize = 10;
        private int totalCount;
        private int totalPages;
        private bool isLoading;
        private string? errorMessage;

        protected override async Task OnParametersSetAsync()
        {
            if (Page < 1) Page = 1;
            if (plans is null || Page != pageNumber)
            {
                await LoadPage(Page);
            }
        }

        private async Task LoadPage(int page)
        {
            isLoading = true;
            errorMessage = null;
            pageNumber = page;

            try
            {
                var result = await ApiService.GetMembershipPlanListAsync<Result<MemberShipPlanListResponseModel>>(pageNumber, pageSize);
                if (result?.IsSuccess == true && result.Data is not null)
                {
                    plans = result.Data.MemberShipPlans;
                    totalCount = result.Data.TotalCount;
                    totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
                    if (totalPages < 1) totalPages = 1;
                }
                else
                {
                    errorMessage = result?.Message ?? "Failed to load membership plans.";
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
