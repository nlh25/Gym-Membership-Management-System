using GMMS.App.Services;
using GMMS.Domain;
using GMMS.Domain.Features.Member.Models;
using Microsoft.AspNetCore.Components;

namespace GMMS.App.Feature.Member
{
    public partial class MemberEdit : ComponentBase
    {
        [Parameter]
        public int Id { get; set; }

        [Inject]
        private ApiService ApiService { get; set; } = null!;

        [Inject]
        private NavigationManager Navigation { get; set; } = null!;

        private UpdateMemberRequestModel request = new();
        private bool isLoading;
        private string? errorMessage;

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;

            try
            {
                var result = await ApiService.GetMemberDetailsAsync<Result<MemberModel>>(Id);
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

        private async Task Update()
        {
            errorMessage = null;

            try
            {
                var result = await ApiService.UpdateMemberAsync<UpdateMemberRequestModel, Result<MemberModel>>(Id, request);
                if (result?.IsSuccess == true)
                {
                    Navigation.NavigateTo("/member-list");
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
        }
    }
}
