using GMMS.Domain.Features.PaymentMethod;
using GMMS.Domain.Features.PaymentMethod.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GMMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentMethodController : BaseController
    {
        private readonly PaymentMethodService _paymentMethodService;

        public PaymentMethodController(PaymentMethodService paymentMethodService)
        {
            _paymentMethodService = paymentMethodService;
        }
        [HttpGet]
        public IActionResult PaymentMethodList([FromQuery] PaymentMethodListRequestModel request)
        {
            var result = _paymentMethodService.GetList(request);
            return Execute(result);
        }
        [HttpGet("{id}")]
        public IActionResult GetPaymentMethod([FromRoute] int id)
        {
            var result = _paymentMethodService.GetById(id);
            return Execute(result);
        }
        [HttpPost]
        public IActionResult CreatePaymentMethod([FromBody] PaymentMethodCreateRequestModel request)
        {
            var result = _paymentMethodService.Create(request);
            return Execute(result);
        }
        [HttpPut("{id}")]
        public IActionResult UpdatePaymentMethod([FromRoute] int id, [FromBody] PaymentMethodUpdateRequestModel request)
        {
            if (id != request.PaymentMethodId)
            {
                return BadRequest("Payment Method ID in the route does not match the ID in the request body.");
            }
            var result = _paymentMethodService.Update(id, request);
            return Execute(result);
        }
        [HttpDelete("{id}")]
        public IActionResult DeletePaymentMethod([FromRoute] int id)
        {
            var result = _paymentMethodService.Delete(id);
            return Execute(result);
        }

    }
}
