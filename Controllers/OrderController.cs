using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TreeStore.Models.Category;
using TreeStore.Models.CustomerModels;
using TreeStore.Models.CustomModels;
using TreeStore.Models.Entities;
using TreeStore.Models.Order;
using TreeStore.Models.ProductModels;
using TreeStore.Models.UserModels;
using TreeStore.Services;
using TreeStore.Services.Interfaces;

namespace TreeStore.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {

        private readonly IOrderServices _ordersService;

        public OrderController(IOrderServices orderService)
        {
            _ordersService = orderService;
        }
        [HttpPost]
        public async Task<ResultCustomModel<int>> Create([FromBody] CreateOrderRequest request)
        {
            return await _ordersService.CreateOrdersAsync(request);
        }



        [HttpGet]
        public async Task<ResultCustomModel<List<GetListOrderSPResult>>> ListOrder()
        {
            return await _ordersService.GetListOrderAsync();
        }

        [HttpGet]
        public async Task<ResultCustomModel<List<GetListOrderByCustomerIdSPResult>>> ListOrderByCustomerId([FromQuery] int customerId)
        {
            return await _ordersService.GetListOrderByCustomerIdAsync(customerId);
        }

        [HttpGet]
        public async Task<ResultCustomModel<DetailOrderReponse>> ListDetailOrder(int orderId)
        {
            return await _ordersService.GetListDetailOrderAsync(orderId);
        }

        [HttpGet]
        public async Task<ResultCustomModel<OrderReponse>> GetOrderById([FromQuery] int orderId)
        {
            return await _ordersService.GetOrderByIdAsync(orderId);
        }
        [HttpPost]
        public async Task<ResultCustomModel<bool>> ChangeStateOrder(int orderId, short stateId)
        {
            return await _ordersService.ChangeStateOrderAsync(orderId, stateId);
        }

        [HttpGet]
        public async Task<ResultCustomModel<List<RevenueResponse>>> GetRevenueLast7Days()
        {
            return await _ordersService.GetRevenueLast7DaysAsync();
        }

        [HttpGet("total-orders")]
        public async Task<ResultCustomModel<int>> GetTotalOrders()
        {
            // Gọi service để tính tổng số đơn hàng
            var result = await _ordersService.GetTotalOrdersAsync();

            // Trả về kết quả (response) cho client
            return result;
        }
        [HttpPost]
        public async Task<ResultCustomModel<bool>> CancelOrder([FromQuery] int orderId)
        {
            var order = await _ordersService.GetOrderByIdAsync(orderId);

            // Kiểm tra trạng thái đơn hàng hiện tại
            if (order.Data.State != 0)
            {
                return new ResultCustomModel<bool>
                {
                    Code = 400,
                    Success = false,
                    Data = false,
                    Message = "Chỉ có thể hủy đơn hàng khi trạng thái là 'Chờ xác nhận'"
                };
            }

            // Gọi lại hàm đổi trạng thái có sẵn
            return await _ordersService.ChangeStateOrderAsync(orderId, 4); // 4 = HỦY
        }

        [HttpPost]
        public async Task<IActionResult> CaptureOrder(int orderId)
        {
            var result = await _ordersService.CaptureOrderAsync(orderId);

            if (!result.Success)
            {
                return StatusCode(result.Code ?? 500, new { success = result.Success, message = result.Message });
            }

            return Ok(new
            {
                success = result.Success,
                message = result.Message,
                data = result.Data,
                code = result.Code
            });
        }
        
    }
}