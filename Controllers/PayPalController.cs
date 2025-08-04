using Microsoft.AspNetCore.Mvc;
using PayPalCheckoutSdk.Orders;
using TreeStore.Models.CustomModels;
using TreeStore.Models.Entities;
using TreeStore.Models.Payment;
using TreeStore.Services;
using TreeStore.Services.Interfaces;

namespace TreeStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PayPalController : ControllerBase
    {
        private readonly IPayPalService _payPalService;

        public PayPalController(IPayPalService payPalService)
        {
            _payPalService = payPalService;
        }

        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            var approvalUrl = await _payPalService.CreateOrder(dto.Amount, "USD", dto.ReturnUrl, dto.CancelUrl, dto.OrderId);
            return Ok(new { ApprovalUrl = approvalUrl });
        }

        [HttpPost("capture-order/{orderId}")]
        public async Task<ResultCustomModel<CaptureOrderResponse>> CaptureOrder(string orderId)
        {
            return  await _payPalService.CaptureOrder(orderId); 
        }
    }
}


public class CreateOrderDto
{
    public decimal Amount { get; set; }
    public string ReturnUrl { get; set; }
    public string CancelUrl { get; set; }
    public string OrderId { get; set; }
}
