using GMMS.App.Services;
using GMMS.Domain;
using GMMS.Domain.Features.PaymentMethod.Models;
using Microsoft.AspNetCore.Components;

namespace GMMS.App.Feature.PaymentMethod
{
    public partial class PaymentMethodEdit : ComponentBase
    {
        [Parameter]
        public int Id { get; set; }

        [Inject]
        private ApiService ApiService { get; set; } = null!;

        [Inject]
        private NavigationManager Navigation { get; set; } = null!;

        private PaymentMethodUpdateRequestModel request = new();
        private bool isLoading;
        private string? errorMessage;

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;

            try
            {
                var result = await ApiService.GetPaymentMethodDetailsAsync<Result<PaymentMethodModel>>(Id);
                if (result?.IsSuccess == true && result.Data is not null)
                {
                    request.PaymentMethodId = result.Data.PaymentMethodId;
                    request.PaymentMethodCode = result.Data.PaymentMethodCode;
                    request.Name = result.Data.Name;
                    request.IsActive = result.Data.IsActive;
                }
                else
                {
                    errorMessage = result?.Message ?? "Payment method not found.";
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
                var result = await ApiService.UpdatePaymentMethodAsync<PaymentMethodUpdateRequestModel, Result<PaymentMethodModel>>(Id, request);
                if (result?.IsSuccess == true)
                {
                    Navigation.NavigateTo("/paymentmethod-list");
                }
                else
                {
                    errorMessage = result?.Message ?? "Failed to update payment method.";
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }
    }
}
