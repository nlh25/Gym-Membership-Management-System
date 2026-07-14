using GMMS.App.Services;
using GMMS.Domain;
using GMMS.Domain.Features.PaymentMethod.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Text.RegularExpressions;

namespace GMMS.App.Feature.PaymentMethod
{
    public partial class PaymentMethodCreate : ComponentBase
    {
        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; } = null!;

        [Inject]
        private ApiService ApiService { get; set; } = null!;

        private PaymentMethodCreateRequestModel request = new();
        private bool isSaving;
        private string? errorMessage;
        private List<string> validationErrors = new();

        private static readonly Regex CodeRegex = new("^[A-Z0-9-]+$", RegexOptions.Compiled);

        private void Cancel()
        {
            MudDialog.Cancel();
        }

        private void OnCodeChanged(string value)
        {
            request.PaymentMethodCode = value?.ToUpperInvariant() ?? "";
            ValidateCode();
        }

        private void OnNameChanged(string value)
        {
            request.Name = value?.Trim() ?? "";
            ValidateName();
        }

        private void ValidateCode()
        {
            validationErrors.RemoveAll(e => e.StartsWith("Code"));
            if (string.IsNullOrWhiteSpace(request.PaymentMethodCode))
            {
                validationErrors.Add("Code is required.");
                return;
            }
            if (request.PaymentMethodCode.Length > 50)
                validationErrors.Add("Code must not exceed 50 characters.");
            if (!CodeRegex.IsMatch(request.PaymentMethodCode))
                validationErrors.Add("Code can only contain uppercase letters, numbers, and hyphens.");
        }

        private void ValidateName()
        {
            validationErrors.RemoveAll(e => e.StartsWith("Name"));
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                validationErrors.Add("Name is required.");
                return;
            }
            if (request.Name.Length > 100)
                validationErrors.Add("Name must not exceed 100 characters.");
        }

        private bool ValidateAll()
        {
            validationErrors.Clear();
            ValidateCode();
            ValidateName();

            errorMessage = validationErrors.Count > 0 ? string.Join(" ", validationErrors) : null;
            return validationErrors.Count == 0;
        }

        private async Task Save()
        {
            if (!ValidateAll())
            {
                return;
            }

            isSaving = true;
            errorMessage = null;

            try
            {
                var result = await ApiService.CreatePaymentMethodAsync<PaymentMethodCreateRequestModel, Result<PaymentMethodModel>>(request);

                if (result?.IsSuccess == true)
                {
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