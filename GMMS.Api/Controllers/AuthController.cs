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
        private readonly ILogger<AuthController> _logger;

        public AuthController(AuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task <IActionResult> Login([FromBody] LoginRequestModel request)
        {
            _logger.LogInformation("Login API called. UserName={UserName}", request.UserName);

            var result = await _authService.Login(request);
            if (result.IsSuccess)
            {
                _logger.LogInformation("Login API completed successfully. UserName={UserName}", request.UserName);
            }
            else
            {
                _logger.LogWarning("Login API failed. UserName={UserName}, Message={Message}", request.UserName, result.Message);
            }
            return Execute(result);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task <IActionResult> ChangePassword([FromBody] ChangePasswordRequestModel request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            _logger.LogInformation("ChangePassword API called. UserId={UserId}", userId);

            var result = await _authService.ChangePassword(userId, request);
            if (result.IsSuccess)
            {
                _logger.LogInformation("ChangePassword API completed successfully. UserId={UserId}", userId);
            }
            else
            {
                _logger.LogWarning("ChangePassword API failed. UserId={UserId}, Message={Message}", userId, result.Message);
            }
            return Execute(result);
        }
    }
}
