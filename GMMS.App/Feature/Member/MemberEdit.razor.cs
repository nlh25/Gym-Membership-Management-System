using GMMS.App.Services;
using GMMS.Domain;
using GMMS.Domain.Features.Member.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

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

        private void Cancel()
        {
            MudDialog.Cancel();
        }

        private async Task Update()
        {
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
