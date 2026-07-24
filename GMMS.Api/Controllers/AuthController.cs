using GMMS.Domain.Features.Auth;
using GMMS.Domain.Features.Auth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GMMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task <IActionResult> Login([FromBody] LoginRequestModel request)
        {
            var result = await _authService.Login(request);
            return Execute(result);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task <IActionResult> ChangePassword([FromBody] ChangePasswordRequestModel request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await _authService.ChangePassword(userId, request);
            return Execute(result);
        }
    }
}
