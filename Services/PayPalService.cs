using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using System.Globalization;
using TreeStore.Models.CustomModels;
using TreeStore.Models.Payment;
using TreeStore.Services.Interfaces;

namespace TreeStore.Services
{
    public class PayPalService : BaseServices, IPayPalService
    {
        private readonly PayPalHttpClient _client;

        public PayPalService(Models.Entities.TreeStoreDBContext db, Models.Entities.ITreeStoreDBContextProcedures sp, IConfiguration configuration) : base(db, sp)
        {
            var clientId = configuration["PayPal:ClientId"];
            var clientSecret = configuration["PayPal:ClientSecret"];
            var environment = new SandboxEnvironment(clientId, clientSecret);

            _client = new PayPalHttpClient(environment);
        }

        public async Task<string> CreateOrder(decimal amount, string currency, string returnUrl, string cancelUrl, string orderId)
        {
            var orderRequest = new OrderRequest()
            {
                CheckoutPaymentIntent = "CAPTURE", // Yêu cầu thanh toán trực tiếp
                ApplicationContext = new ApplicationContext
                {
                    ReturnUrl = returnUrl,  // URL khi thanh toán thành công
                    CancelUrl = cancelUrl   // URL khi người dùng hủy bỏ thanh toán
                },
                PurchaseUnits = new List<PurchaseUnitRequest>
        {
            new PurchaseUnitRequest
            {
                AmountWithBreakdown = new AmountWithBreakdown
                {
                    CurrencyCode = currency,  // Tiền tệ
                    Value = amount.ToString("F2", CultureInfo.InvariantCulture)  // Số tiền thanh toán (định dạng có 2 chữ số thập phân)
                },
                InvoiceId = orderId
            }
        }
            };

            var request = new OrdersCreateRequest();
            request.Prefer("return=representation");  // Nhận kết quả chi tiết
            request.RequestBody(orderRequest);

            try
            {
                var response = await _client.Execute(request);  // Gửi yêu cầu tạo đơn hàng
                var result = response.Result<Order>();

                // Kiểm tra nếu phản hồi trả về thành công và chứa link "approve"
                var approveLink = result.Links.FirstOrDefault(link => link.Rel == "approve")?.Href;

                if (approveLink == null)
                {
                    throw new Exception("Approve link not found in PayPal response.");
                }

                return approveLink;  // Trả về link thanh toán cho người dùng
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                Console.WriteLine($"Error during order creation: {ex.Message}");
                throw;
            }
        }

        public async Task<ResultCustomModel<CaptureOrderResponse>> CaptureOrder(string orderId)
        {
            var request = new OrdersCaptureRequest(orderId);
            request.RequestBody(new OrderActionRequest());

            try
            {
                // Kiểm tra trạng thái đơn hàng trước khi thực hiện capture
                var orderStatus = await GetOrderStatus(orderId);
                var orderIdSystem = orderStatus.PurchaseUnits.FirstOrDefault().InvoiceId;
                TreeStore.Models.Entities.Order order = _db.Orders.FirstOrDefault(x => x.OrderId == Int32.Parse(orderIdSystem ?? "0"));
                order.IsPaid = true;
                _db.Entry(order).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _db.SaveChanges();
                if (orderStatus.Status == "COMPLETED")  // Nếu đơn hàng đã được capture
                {
                    throw new Exception("Order has already been captured.");
                }

                // Thực hiện capture đơn hàng
                var responses = await _client.Execute(request);
                return new ResultCustomModel<CaptureOrderResponse>
                {
                    Code = 200,
                    Data = new CaptureOrderResponse()
                    {
                        OrderId = order.OrderId,
                        TotalAmount = order.TotalAmount
                    },
                    Success = true,
                    Message = "Thanh toán đơn hàng thành công"
                };
            }
            catch (PayPalHttp.HttpException ex)
            {
                // Xử lý lỗi HTTP từ PayPal, như ORDER_ALREADY_CAPTURED
                Console.WriteLine($"PayPal error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                // Xử lý các lỗi khác
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        // Hàm lấy trạng thái đơn hàng từ PayPal
        public async Task<Order> GetOrderStatus(string orderId)
        {
            var request = new OrdersGetRequest(orderId);
            var response = await _client.Execute(request);
            var order = response.Result<Order>();

            return order;  // Trả về trạng thái của đơn hàng
        }
    }
}
