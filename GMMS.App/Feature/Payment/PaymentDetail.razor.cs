using GMMS.App.Services;
using GMMS.Domain;
using GMMS.Domain.Features.Payment.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GMMS.App.Feature.Payment
{
    public partial class PaymentDetail : ComponentBase
    {
        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; } = null!;

        [Parameter]
        public int PaymentId { get; set; }

        [Inject]
        private ApiService ApiService { get; set; } = null!;

        private PaymentDetailModel? detail;
        private bool isLoading = true;
        private string? errorMessage;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var result = await ApiService.GetPaymentDetailsAsync<Result<PaymentDetailModel>>(PaymentId);
                if (result?.IsSuccess == true && result.Data is not null)
                {
                    detail = result.Data;
                }
                else
                {
                    errorMessage = result?.Message ?? "Payment not found.";
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
    }
}