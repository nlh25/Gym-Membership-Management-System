using GMMS.App.Services;
using GMMS.Domain;
using GMMS.Domain.Features.PaymentMethod.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GMMS.App.Feature.PaymentMethod
{
    public partial class PaymentMethodCreate : ComponentBase
    {
        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; } = null!;

        [Inject]
        private ApiService ApiService { get; set; } = null!;

        [Inject]
        private ISnackbar Snackbar { get; set; } = null!;

        private PaymentMethodCreateRequestModel request = new();
        private bool isSaving;
        private string? errorMessage;

       

        private void OnCodeChanged(string value)
        {
            request.PaymentMethodCode = value?.ToUpperInvariant() ?? "";
        }

        private void Cancel()
        {
            MudDialog.Cancel();
        }

        private async Task Save()
        {
            isSaving = true;
            errorMessage = null;

            try
            {
                var result = await ApiService.CreatePaymentMethodAsync<PaymentMethodCreateRequestModel, Result<PaymentMethodModel>>(request);

                if (result?.IsSuccess == true)
                {
                    Snackbar.Add("Payment method created successfully!", Severity.Success);
                    MudDialog.Close(DialogResult.Ok(true));
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
                isSaving = false;
            }
        }
    }
}