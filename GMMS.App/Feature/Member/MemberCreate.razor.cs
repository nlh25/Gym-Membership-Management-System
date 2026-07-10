using GMMS.App.Services;
using GMMS.Domain;
using GMMS.Domain.Features.Member.Models;
using Microsoft.AspNetCore.Components;

namespace GMMS.App.Feature.Member
{
    public partial class MemberCreate : ComponentBase
    {
        [Inject]
        private ApiService ApiService { get; set; } = null!;

        [Inject]
        private NavigationManager Navigation { get; set; } = null!;

        private CreateMemberRequestModel request = new();
        private bool isLoading;
        private string? errorMessage;
        

        private async Task Save()
        {
            isLoading = true;
            errorMessage = null;
            

            try
            {
               
                var result = await ApiService.CreateMemberAsync<CreateMemberRequestModel, Result<MemberModel>>(request);

                

                if (result?.IsSuccess == true)
                {
                    Navigation.NavigateTo("/member-list");
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
