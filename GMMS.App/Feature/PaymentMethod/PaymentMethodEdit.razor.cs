using GMMS.App.Services;
using GMMS.Domain;
using GMMS.Domain.Features.PaymentMethod.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GMMS.App.Feature.PaymentMethod
{
    public partial class PaymentMethodEdit : ComponentBase
    {
        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; } = null!;

        [Parameter]
        public int PaymentMethodId { get; set; }

        [Inject]
        private ApiService ApiService { get; set; } = null!;

        private PaymentMethodUpdateRequestModel request = new();
        private bool isLoading = true;
        private bool isSaving;
        private string? errorMessage;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var result = await ApiService.GetPaymentMethodDetailsAsync<Result<PaymentMethodModel>>(PaymentMethodId);
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

        private void Cancel()
        {
            MudDialog.Cancel();
        }

        private async Task Update()
        {
            errorMessage = null;

            try
            {
                var result = await ApiService.UpdatePaymentMethodAsync<PaymentMethodUpdateRequestModel, Result<PaymentMethodModel>>(PaymentMethodId, request);
                if (result?.IsSuccess == true)
                {
                    MudDialog.Close(DialogResult.Ok(true));
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