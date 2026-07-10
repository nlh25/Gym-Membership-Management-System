using GMMS.App.Services;
using GMMS.Domain;
using GMMS.Domain.Features.PaymentMethod.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace GMMS.App.Feature.PaymentMethod
{
    public partial class PaymentMethodDelete : ComponentBase
    {
        [Parameter]
        public int Id { get; set; }

        [Inject]
        private ApiService ApiService { get; set; } = null!;

        [Inject]
        private NavigationManager Nav { get; set; } = null!;

        private PaymentMethodModel? method;
        private bool isLoading = true;
        private bool isDeleting;
        private string? errorMessage;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var result = await ApiService.GetPaymentMethodDetailsAsync<Result<PaymentMethodModel>>(Id);
                if (result?.IsSuccess == true && result.Data is not null)
                {
                    method = result.Data;
                }
                else
                {
                    errorMessage = result?.Message ?? "Failed to load payment method.";
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

        private async Task ConfirmDelete()
        {
            isDeleting = true;
            errorMessage = null;
            StateHasChanged();

            try
            {
                var result = await ApiService.DeletePaymentMethodAsync<Result<bool>>(Id);
                if (result?.IsSuccess == true)
                {
                    Nav.NavigateTo("paymentmethod-list");
                }
                else
                {
                    errorMessage = result?.Message ?? "Failed to delete payment method.";
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            finally
            {
                isDeleting = false;
            }
        }
    }
}
