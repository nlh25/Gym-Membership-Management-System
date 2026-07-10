using GMMS.Domain.Features.Payment;
using GMMS.Domain.Features.Payment.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GMMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : BaseController
    {
        private readonly PaymentService _paymentService;
        public PaymentController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }
        [HttpGet]
        public IActionResult PaymentList([FromQuery] PaymentListRequestModel request)
        {
            var result = _paymentService.GetList(request);
            return Execute(result);
        }
        [HttpGet("{id}")]
        public IActionResult GetPaymentDetail(int id)
        {
            var result = _paymentService.GetById(id);
            return Execute(result);
        }
        [HttpPost]
        public IActionResult CreatePayment([FromBody] CreatePaymentRequestModel request)
        {
            var result = _paymentService.Create(request);
            return Execute(result);
        }
    }
}
