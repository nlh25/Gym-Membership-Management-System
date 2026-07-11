using GMMS.App.Services;
using GMMS.Domain;
using GMMS.Domain.Features.MemberShip.Models;
using Microsoft.AspNetCore.Components;

namespace GMMS.App.Feature.Membership
{
    public partial class MembershipList : ComponentBase
    {
        [Inject]
        private ApiService ApiService { get; set; } = null!;

        [Inject]
        private NavigationManager Navigation { get; set; } = null!;

        [SupplyParameterFromQuery(Name = "page")]
        public int Page { get; set; } = 1;

        [SupplyParameterFromQuery(Name = "memberId")]
        public int MemberId { get; set; }

        private List<MemberShipModel>? memberships;
        private int pageNumber = 1;
        private int pageSize = 10;
        private int totalCount;
        private int totalPages;
        private bool isLoading;
        private string? errorMessage;

        protected override async Task OnParametersSetAsync()
        {
            if (Page < 1) Page = 1;
            if (MemberId <= 0)
            {
                Navigation.NavigateTo("/member-list");
                return;
            }
            if (memberships is null || Page != pageNumber)
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
                var result = await ApiService.GetMembershipListAsync<Result<MemberShipListResponseModel>>(MemberId, pageNumber, pageSize);
                if (result?.IsSuccess == true && result.Data is not null)
                {
                    memberships = result.Data.MemberShips;
                    totalCount = result.Data.TotalCount;
                    totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
                    if (totalPages < 1) totalPages = 1;
                }
                else
                {
                    errorMessage = result?.Message ?? "Failed to load memberships.";
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
