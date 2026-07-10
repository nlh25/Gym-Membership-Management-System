using GMMS.App.Services;
using GMMS.Domain;
using GMMS.Domain.Features.Payment.Models;
using Microsoft.AspNetCore.Components;

namespace GMMS.App.Feature.Payment
{
    public partial class PaymentList : ComponentBase
    {
        [Inject]
        private ApiService ApiService { get; set; } = null!;

        [SupplyParameterFromQuery(Name = "page")]
        public int Page { get; set; } = 1;

        private List<PaymentModel>? payments;
        private int pageNumber = 1;
        private int pageSize = 10;
        private int totalCount;
        private int totalPages;
        private bool isLoading;
        private string? errorMessage;

        protected override async Task OnParametersSetAsync()
        {
            if (Page < 1) Page = 1;
            if (payments is null || Page != pageNumber)
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
                var result = await ApiService.GetPaymentListAsync<Result<PaymentListResponseModel>>(pageNumber, pageSize);
                if (result?.IsSuccess == true && result.Data is not null)
                {
                    payments = result.Data.Payments;
                    totalCount = result.Data.TotalCount;
                    totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
                    if (totalPages < 1) totalPages = 1;
                }
                else
                {
                    errorMessage = result?.Message ?? "Failed to load payments.";
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
