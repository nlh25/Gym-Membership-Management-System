using GMMS.App.Services;
using GMMS.Domain;
using GMMS.Domain.Features.Payment.Models;
using Microsoft.AspNetCore.Components;

namespace GMMS.App.Feature.Payment
{
    public partial class PaymentCreate : ComponentBase
    {
        [Inject]
        private ApiService ApiService { get; set; } = null!;

        [Inject]
        private NavigationManager Navigation { get; set; } = null!;

        private CreatePaymentRequestModel request = new();
        private bool isLoading;
        private string? errorMessage;

        private async Task Save()
        {
            isLoading = true;
            errorMessage = null;

            try
            {
                var result = await ApiService.CreatePaymentAsync<CreatePaymentRequestModel, Result<PaymentModel>>(request);

                if (result?.IsSuccess == true)
                {
                    Navigation.NavigateTo("/payment-list");
                }
                else
                {
                    errorMessage = result?.Message ?? "Failed to create payment.";
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
