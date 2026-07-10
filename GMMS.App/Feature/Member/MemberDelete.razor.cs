using GMMS.App.Services;
using GMMS.Domain;
using GMMS.Domain.Features.Member.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace GMMS.App.Feature.Member
{
    public partial class MemberDelete : ComponentBase
    {
        [Parameter]
        public int Id { get; set; }

        [Inject]
        private ApiService ApiService { get; set; } = null!;

        [Inject]
        private NavigationManager Nav { get; set; } = null!;

        private MemberModel? member;
        private bool isLoading = true;
        private bool isDeleting;
        private string? errorMessage;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var result = await ApiService.GetMemberDetailsAsync<Result<MemberModel>>(Id);
                if (result?.IsSuccess == true && result.Data is not null)
                {
                    member = result.Data;
                }
                else
                {
                    errorMessage = result?.Message ?? "Failed to load member.";
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
                var result = await ApiService.DeleteMemberAsync<Result<bool>>(Id);
                if (result?.IsSuccess == true)
                {
                    Nav.NavigateTo("member-list");
                }
                else
                {
                    errorMessage = result?.Message ?? "Failed to delete member.";
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
