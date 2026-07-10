using GMMS.App.Services;
using GMMS.Domain;
using GMMS.Domain.Features.PaymentMethod.Models;
using Microsoft.AspNetCore.Components;

namespace GMMS.App.Feature.PaymentMethod
{
    public partial class PaymentMethodCreate : ComponentBase
    {
        [Inject]
        private ApiService ApiService { get; set; } = null!;

        [Inject]
        private NavigationManager Navigation { get; set; } = null!;

        private PaymentMethodCreateRequestModel request = new();
        private bool isLoading;
        private string? errorMessage;

        private async Task Save()
        {
            isLoading = true;
            errorMessage = null;

            try
            {
                var result = await ApiService.CreatePaymentMethodAsync<PaymentMethodCreateRequestModel, Result<PaymentMethodModel>>(request);

                if (result?.IsSuccess == true)
                {
                    Navigation.NavigateTo("/paymentmethod-list");
                }
                else
                {
                    errorMessage = result?.Message ?? "Failed to create payment method.";
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
