using GMMS.App.Services;
using GMMS.Domain;
using GMMS.Domain.Features.Member.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Text.RegularExpressions;

namespace GMMS.App.Feature.Member
{
    public partial class MemberEdit : ComponentBase
    {
        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; } = null!;

        [Parameter]
        public int MemberId { get; set; }

        [Inject]
        private ApiService ApiService { get; set; } = null!;

        private UpdateMemberRequestModel request = new();
        private bool isLoading = true;
        private bool isSaving;
        private string? errorMessage;
        private List<string> validationErrors = new();

        private const string MemberCodePattern = "^[A-Z0-9-]+$";
        private const int MemberCodeMaxLength = 50;
        private const int NameMaxLength = 100;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var result = await ApiService.GetMemberDetailsAsync<Result<MemberModel>>(MemberId);
                if (result?.IsSuccess == true && result.Data is not null)
                {
                    request.MemberId = result.Data.MemberId;
                    request.MemberCode = result.Data.MemberCode;
                    request.Name = result.Data.Name;
                }
                else
                {
                    errorMessage = result?.Message ?? "Member not found.";
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
                validationErrors.Add("Member Code: Member code is required.");
                return;
            }

            if (request.MemberCode.Length > MemberCodeMaxLength)
            {
                validationErrors.Add($"Member Code: Member code must not exceed {MemberCodeMaxLength} characters.");
            }

            if (!Regex.IsMatch(request.MemberCode, MemberCodePattern))
            {
                validationErrors.Add("Member Code: Member code can only contain uppercase letters, numbers, and hyphens.");
            }
        }

        private void ValidateName()
        {
            validationErrors.RemoveAll(e => e.StartsWith("Name"));
            
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                validationErrors.Add("Name: Member name is required.");
                return;
            }

            if (request.Name.Length > NameMaxLength)
            {
                validationErrors.Add($"Name: Member name must not exceed {NameMaxLength} characters.");
            }
        }

        private bool ValidateAll()
        {
            validationErrors.Clear();
            ValidateMemberCode();
            ValidateName();
            
            errorMessage = validationErrors.Count > 0 ? string.Join("\n", validationErrors) : null;
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
                var result = await ApiService.UpdateMemberAsync<UpdateMemberRequestModel, Result<MemberModel>>(MemberId, request);
                if (result?.IsSuccess == true)
                {
                    MudDialog.Close(DialogResult.Ok(true));
                }
                else
                {
                    errorMessage = result?.Message ?? "Failed to update member.";
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
