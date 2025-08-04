using TreeStore.Models.CustomModels;
using TreeStore.Models.LoginModels;
using TreeStore.Models.UserModels;
using TreeStore.Models.CustomerModels;
using static TreeStore.Services.PayPalService;
using PayPalCheckoutSdk.Orders;
using TreeStore.Models.Payment;

namespace TreeStore.Services.Interfaces
{
    public interface IPayPalService
    {

        public Task<string> CreateOrder(decimal amount, string currency, string returnUrl, string cancelUrl, string orderId);
        public Task<ResultCustomModel<CaptureOrderResponse>> CaptureOrder(string orderId);
    }
}
