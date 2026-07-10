using GMMS.App.Services;
using GMMS.Domain;
using GMMS.Domain.Features.Member.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace GMMS.App.Feature.Member
{
    public partial class MemberList : ComponentBase
    {
        [Inject]
        private ApiService ApiService { get; set; } = null!;

        [SupplyParameterFromQuery(Name = "page")]
        public int Page { get; set; } = 1;

        private List<MemberModel>? members;
        private int pageNumber = 1;
        private int pageSize = 10;
        private int totalCount;
        private int totalPages;
        private bool isLoading;
        private string? errorMessage;

        protected override async Task OnParametersSetAsync()
        {
            if (Page < 1) Page = 1;
            if (members is null || Page != pageNumber)
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
                var result = await ApiService.GetMemberListAsync<Result<MemberListResponseModel>>(pageNumber, pageSize);
                if (result?.IsSuccess == true && result.Data is not null)
                {
                    members = result.Data.Members;
                    totalCount = result.Data.TotalCount;
                    totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
                    if (totalPages < 1) totalPages = 1;
                }
                else
                {
                    errorMessage = result?.Message ?? "Failed to load members.";
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
