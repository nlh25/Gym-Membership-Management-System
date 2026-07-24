using GMMS.Domain.Features.User;
using GMMS.Domain.Features.User.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GMMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Owner")]
    public class UserController : BaseController
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task <IActionResult> UserList([FromQuery] UserListRequestModel request)
        {
            var result = await _userService.GetList(request);
            return Execute(result);
        }

        [HttpGet("{id}")]
        public async Task <IActionResult> GetUser([FromRoute] int id)
        {
            var result =await _userService.GetById(id);
            return Execute(result);
        }

        [HttpPost]
        public async Task <IActionResult> CreateUser([FromBody] CreateUserRequestModel request)
        {
            var result = await _userService.Create(request);
            return Execute(result);
        }

        [HttpPut("{id}")]
        public async Task <IActionResult> UpdateUser([FromRoute] int id, [FromBody] UpdateUserRequestModel request)
        {
            if (id != request.UserId)
            {
                return BadRequest("User ID in the route does not match the ID in the request body.");
            }
            var result = await _userService.Update(id, request);
            return Execute(result);
        }

        [HttpPost("reset-password")]
        public async Task <IActionResult> ResetPassword([FromBody] ResetPasswordRequestModel request)
        {
            var result =await _userService.ResetPassword(request);
            return Execute(result);
        }

        [HttpDelete("{id}")]
        public async Task <IActionResult> DeleteUser([FromRoute] int id)
        {
            var result = await _userService.Delete(id);
            return Execute(result);
        }
    }
}
