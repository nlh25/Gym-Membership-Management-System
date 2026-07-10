using GMMS.Domain.Features.MemberShipPlan;
using GMMS.Domain.Features.MemberShipPlan.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GMMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberShipPlanController : BaseController
    {
        private readonly MemberShipPlanService _memberShipPlanService;

        public MemberShipPlanController(MemberShipPlanService memberShipPlanService)
        {
            _memberShipPlanService = memberShipPlanService;
        }
        [HttpGet]
        public IActionResult MemberShipPlanList([FromQuery] MemberShipPlanlistRequestModel request)
        {
            var result = _memberShipPlanService.GetList(request);
            return Execute(result);
        }
        [HttpGet("{id}")]
        public IActionResult MemberShipPlanById([FromRoute] int id)
        {
            var result = _memberShipPlanService.GetById(id);
            return Execute(result);
        }
        [HttpPost]
        public IActionResult CreateMemberShipPlan([FromBody] CreateMemberShipPlanRequestModel request)
        {
            var result = _memberShipPlanService.Create(request);
            return Execute(result);
        }
        [HttpPut("{id}")]
        public IActionResult UpdateMemberShipPlan([FromRoute] int id, [FromBody] UpdateMemberShipPlanRequestModel request)
        {
            if (id != request.MemberShipPlanId)
            {
                return BadRequest("Member ID in the route does not match the ID in the request body.");
            }
            var result = _memberShipPlanService.Update(id, request);
            return Execute(result);
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteMemberShipPlan([FromRoute] int id)
        {
            var result = _memberShipPlanService.Delete(id);
            return Execute(result);
        }
    }
}
