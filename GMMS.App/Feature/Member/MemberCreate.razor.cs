using GMMS.App.Services;
using GMMS.Domain;
using GMMS.Domain.Features.Member.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Text.RegularExpressions;

namespace GMMS.App.Feature.Member
{
    public partial class MemberCreate : ComponentBase
    {
        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; } = null!;

        [Inject]
        private ApiService ApiService { get; set; } = null!;

        private CreateMemberRequestModel request = new();
        private bool isLoading;
        private string? errorMessage;
        private List<string> validationErrors = new();

        private static readonly Regex MemberCodeRegex = new("^[A-Z0-9-]+$", RegexOptions.Compiled);
        private const int MemberCodeMaxLength = 50;
        private const int NameMaxLength = 100;

        private void Cancel()
        {
            MudDialog.Cancel();
        }

        private void OnMemberCodeChanged(string value)
        {
            request.MemberCode = value?.ToUpperInvariant() ?? "";
            ValidateMemberCode();
        }

        private void OnNameChanged(string value)
        {
            request.Name = value ?? "";
            ValidateName();
        }

        private void ValidateMemberCode()
        {
            validationErrors.RemoveAll(e => e.StartsWith("Member Code"));

            if (string.IsNullOrWhiteSpace(request.MemberCode))
            {
                validationErrors.Add("Member Code is required.");
                return;
            }

            if (request.MemberCode.Length > MemberCodeMaxLength)
            {
                validationErrors.Add($"Member Code must not exceed {MemberCodeMaxLength} characters.");
            }

            if (!MemberCodeRegex.IsMatch(request.MemberCode))
            {
                validationErrors.Add("Member Code can only contain uppercase letters, numbers, and hyphens.");
            }
        }

        private void ValidateName()
        {
            validationErrors.RemoveAll(e => e.StartsWith("Name"));

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                validationErrors.Add("Name is required.");
                return;
            }

            if (request.Name.Length > NameMaxLength)
            {
                validationErrors.Add($"Name must not exceed {NameMaxLength} characters.");
            }
        }

        private bool ValidateAll()
        {
            validationErrors.Clear();
            ValidateMemberCode();
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

            isLoading = true;
            errorMessage = null;

            try
            {
                var result = await ApiService.CreateMemberAsync<CreateMemberRequestModel, Result<MemberModel>>(request);

                if (result?.IsSuccess == true)
                {
                    MudDialog.Close(DialogResult.Ok(true));
                }
                else
                {
                    errorMessage = result?.Message ?? "Failed to create member.";
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
