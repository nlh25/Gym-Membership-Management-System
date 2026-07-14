using GMMS.App.Services;
using GMMS.Domain;
using GMMS.Domain.Features.MemberShipPlan.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Text.RegularExpressions;

namespace GMMS.App.Feature.MembershipPlan
{
    public partial class MembershipPlanEdit : ComponentBase
    {
        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; } = null!;

        [Parameter]
        public int PlanId { get; set; }

        [Inject]
        private ApiService ApiService { get; set; } = null!;

        private UpdateMemberShipPlanRequestModel request = new();
        private bool isLoading = true;
        private bool isSaving;
        private string? errorMessage;
        private List<string> validationErrors = new();

        private static readonly Regex CodeRegex = new("^[A-Z0-9-]+$", RegexOptions.Compiled);

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var result = await ApiService.GetMembershipPlanDetailsAsync<Result<MembershipPlanDetailModel>>(PlanId);
                if (result?.IsSuccess == true && result.Data is not null)
                {
                    request.MemberShipPlanId = result.Data.MemberShipPlanId;
                    request.PlanCode = result.Data.PlanCode;
                    request.PlanName = result.Data.PlanName;
                    request.Price = result.Data.Price;
                    request.DurationDays = result.Data.DurationDays;
                    request.Description = result.Data.Description;
                }
                else
                {
                    errorMessage = result?.Message ?? "Membership plan not found.";
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

        private void OnPlanCodeChanged(string value)
        {
            request.PlanCode = value?.ToUpperInvariant() ?? "";
            ValidatePlanCode();
        }

        private void OnPlanNameChanged(string value)
        {
            request.PlanName = value?.Trim() ?? "";
            ValidatePlanName();
        }

        private void OnDescriptionChanged(string value)
        {
            request.Description = value?.Trim();
            ValidateDescription();
        }

        private void OnDurationDaysChanged(int value)
        {
            request.DurationDays = value;
            ValidateDurationDays();
        }

        private void OnPriceChanged(decimal value)
        {
            request.Price = value;
            ValidatePrice();
        }

        private void ValidatePlanCode()
        {
            validationErrors.RemoveAll(e => e.StartsWith("Plan Code"));
            if (string.IsNullOrWhiteSpace(request.PlanCode))
            {
                validationErrors.Add("Plan Code is required.");
                return;
            }
            if (request.PlanCode.Length > 50)
                validationErrors.Add("Plan Code must not exceed 50 characters.");
            if (!CodeRegex.IsMatch(request.PlanCode))
                validationErrors.Add("Plan Code can only contain uppercase letters, numbers, and hyphens.");
        }

        private void ValidatePlanName()
        {
            validationErrors.RemoveAll(e => e.StartsWith("Plan Name"));
            if (string.IsNullOrWhiteSpace(request.PlanName))
            {
                validationErrors.Add("Plan Name is required.");
                return;
            }
            if (request.PlanName.Length > 100)
                validationErrors.Add("Plan Name must not exceed 100 characters.");
        }

        private void ValidateDescription()
        {
            validationErrors.RemoveAll(e => e.StartsWith("Description"));
            if (!string.IsNullOrEmpty(request.Description) && request.Description.Length > 500)
                validationErrors.Add("Description must not exceed 500 characters.");
        }

        private void ValidateDurationDays()
        {
            validationErrors.RemoveAll(e => e.StartsWith("Duration"));
            if (request.DurationDays <= 0)
                validationErrors.Add("Duration must be at least 1 day.");
            else if (request.DurationDays > 3650)
                validationErrors.Add("Duration cannot exceed 10 years (3650 days).");
        }

        private void ValidatePrice()
        {
            validationErrors.RemoveAll(e => e.StartsWith("Price"));
            if (request.Price <= 0)
                validationErrors.Add("Price must be greater than 0.");
            else if (request.Price > 1000000)
                validationErrors.Add("Price cannot exceed 1,000,000.");
        }

        private bool ValidateAll()
        {
            validationErrors.Clear();
            ValidatePlanCode();
            ValidatePlanName();
            ValidateDescription();
            ValidateDurationDays();
            ValidatePrice();

            errorMessage = validationErrors.Count > 0 ? string.Join(" ", validationErrors) : null;
            return validationErrors.Count == 0;
        }

        private void Cancel()
        {
            MudDialog.Cancel();
        }

        private async Task Update()
        {
            if (!ValidateAll())
            {
                return;
            }

            isSaving = true;
            errorMessage = null;

            try
            {
                var result = await ApiService.UpdateMembershipPlanAsync<UpdateMemberShipPlanRequestModel, Result<MemberShipPlanModel>>(PlanId, request);
                if (result?.IsSuccess == true)
                {
                    MudDialog.Close(DialogResult.Ok(true));
                }
                else
                {
                    errorMessage = result?.Message ?? "Failed to update membership plan.";
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