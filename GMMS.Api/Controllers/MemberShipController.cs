using GMMS.Domain.Features.MemberShip;
using GMMS.Domain.Features.MemberShip.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GMMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberShipController : BaseController
    {
        private readonly MemberShipService _memberShipService;
        public MemberShipController(MemberShipService memberShipService)
        {
            _memberShipService = memberShipService;
        }
        [HttpGet]
        public IActionResult Memberlist([FromQuery] MemberShipListRequestModel request)
        {
            var result = _memberShipService.GetList(request);
            return Execute(result);
        }
        [HttpGet("{id}")]
        public IActionResult GetMemberShipById([FromRoute] int id)
        {
            var result = _memberShipService.GetById(id);
            return Execute(result);
        }
        [HttpPost]
        public IActionResult CreateMemberShip([FromBody] CreateMemberShipRequestModel request)
        {
            var result = _memberShipService.Create(request);
            return Execute(result);
        }
        [HttpPut("{id}")]
        public IActionResult UpdateMemberShip([FromRoute] int id, [FromBody] UpdateMembershipRequestModel request)
        {
            if(id != request.MembershipId)
            {
                return BadRequest("Membership ID in the route does not match the ID in the request body.");
            }
            var result = _memberShipService.Update(request);
            return Execute(result);
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteMemberShip([FromRoute] int id)
        {
            var result = _memberShipService.Delete(id);
            return Execute(result);
        }
    }
}
