using GMMS.App.Services;
using GMMS.Domain;
using GMMS.Domain.Features.PaymentMethod.Models;
using Microsoft.AspNetCore.Components;

namespace GMMS.App.Feature.PaymentMethod
{
    public partial class PaymentMethodList : ComponentBase
    {
        [Inject]
        private ApiService ApiService { get; set; } = null!;

        [SupplyParameterFromQuery(Name = "page")]
        public int Page { get; set; } = 1;

        private List<PaymentMethodModel>? methods;
        private int pageNumber = 1;
        private int pageSize = 10;
        private int totalCount;
        private int totalPages;
        private bool isLoading;
        private string? errorMessage;

        protected override async Task OnParametersSetAsync()
        {
            if (Page < 1) Page = 1;
            if (methods is null || Page != pageNumber)
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
                var result = await ApiService.GetPaymentMethodListAsync<Result<PaymentMethodListResponseModel>>(pageNumber, pageSize);
                if (result?.IsSuccess == true && result.Data is not null)
                {
                    methods = result.Data.PaymentMethods;
                    totalCount = result.Data.TotalCount;
                    totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
                    if (totalPages < 1) totalPages = 1;
                }
                else
                {
                    errorMessage = result?.Message ?? "Failed to load payment methods.";
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
