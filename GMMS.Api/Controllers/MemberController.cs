using GMMS.Domain.Features.Member;
using GMMS.Domain.Features.Member.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GMMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : BaseController
    {
        private readonly MemberService _memberService;

        public MemberController(MemberService memberService) 
        {
            _memberService = memberService;
        }
        [HttpGet]
        public IActionResult MemberList([FromQuery] MemberListRequestModel request)
        {
            var result = _memberService.GetList(request); 
            return Execute(result);
        }
        [HttpGet("{id}")]
        public IActionResult GetMemberById([FromRoute] int id)
        {
            var result = _memberService.GetById(id);
            return Execute(result);
        }
        [HttpPost]
        public IActionResult CreateMember([FromBody] CreateMemberRequestModel request)
        {
            var result = _memberService.Create(request);
            return Execute(result);
        }
        [HttpPut("{id}")]
        public IActionResult UpdateMember([FromRoute] int id, [FromBody] UpdateMemberRequestModel request)
        {
            if (id != request.MemberId)
            {
                return BadRequest("Member ID in the route does not match the ID in the request body.");
            }
            var result = _memberService.Update(id, request);
            return Execute(result);
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteMember([FromRoute] int id)
        {
            var result = _memberService.Delete(id);
            return Execute(result);
        }
    }
}
