using TreeStore.Models.Order;
using TreeStore.Models.CustomModels;
using TreeStore.Models.Entities;
using TreeStore.Models.UserModels;
using Microsoft.AspNetCore.Mvc;
namespace TreeStore.Controllers
{
    public interface IOrderServices
    {
        Task<ResultCustomModel<int>> CreateOrdersAsync(CreateOrderRequest request);
        Task<ResultCustomModel<bool>> ChangeStateOrderAsync(int orderId, short stateId);
        Task<ResultCustomModel<OrderReponse>> GetOrderByIdAsync(int orderId);
        Task<ResultCustomModel<List<GetListOrderSPResult>>> GetListOrderAsync();
        Task<ResultCustomModel<List<GetListOrderByCustomerIdSPResult>>> GetListOrderByCustomerIdAsync(int customerId);
        Task<ResultCustomModel<DetailOrderReponse>> GetListDetailOrderAsync(int orderId);
        Task<ResultCustomModel<List<RevenueResponse>>> GetRevenueLast7DaysAsync();
        Task<ResultCustomModel<int>> GetTotalOrdersAsync();
        Task<ResultCustomModel<bool>> CaptureOrderAsync(int orderId);

        //Task<ResultCustomModel<string>> CreateOrderAndGetZaloPayUrlAsync(CreateOrderRequest request);
    }
}
 