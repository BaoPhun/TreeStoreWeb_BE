using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TreeStore.Controllers;
using TreeStore.Models.CustomModels;
using TreeStore.Models.Entities;
using TreeStore.Models.Order;
using TreeStore.Services.Mail;
using TreeStore.Models.Payment;
using TreeStore.Models.UserModels;
using TreeStore.Models.CustomerModels;

namespace TreeStore.Services
{
    public class OrderServices : BaseServices, IOrderServices
    {
        private readonly IEmailService _emailService;


        public OrderServices(TreeStoreDBContext db, ITreeStoreDBContextProcedures sp, IEmailService emailService)
    : base(db, sp)
        {
            _emailService = emailService;
        }

        public async Task<ResultCustomModel<int>> CreateOrdersAsync(CreateOrderRequest request)
        {
            IEnumerable<Product> lstProducts = _db.Products.ToList();
            decimal totalBill = request.CartItems.Sum(cartItem =>
            {
                // Tìm sản phẩm dựa trên ProductId
                var product = lstProducts.FirstOrDefault(p => p.ProductId == cartItem.ProductId);

                // Nếu tìm thấy sản phẩm, tính giá trị của mục giỏ hàng
                if (product != null)
                {
                    return product.PriceOutput * cartItem.Quantity;
                }

                return 0; // Nếu không tìm thấy sản phẩm, giá trị sẽ là 0
            });

            decimal newTotalPrice = totalBill;

            if (!string.IsNullOrEmpty(request.PromotionCode))
            {
                var promotion = await _db.Promotions
                    .FirstOrDefaultAsync(p => p.PromotionCode == request.PromotionCode && p.IsActive && p.EndDate >= DateTime.UtcNow);

                if (promotion != null)
                {
                    var donHangDaMua = _db.Orders
                        .Where(x => x.CustomerId == request.CustomerId && x.PromotionId == promotion.PromotionId)
                        .ToList();

                    if (donHangDaMua.Count < promotion.UsageLimit && promotion.MinimumPurchaseAmount <= totalBill)
                    {
                        newTotalPrice = totalBill - promotion.DiscountAmount;
                        request.PromotionId = promotion.PromotionId;
                    }
                }
            }

            var newOrder = new TreeStore.Models.Entities.Order
            {
                CustomerId = request.CustomerId,
                State = 0,
                Note = request.Note,
                CreateOn = DateTime.UtcNow,
                TotalAmount = newTotalPrice,
                PromotionId = request.PromotionId,
            };

            _db.Orders.Add(newOrder);
            await _db.SaveChangesAsync();

            List<ProductOrder> lstProductOrder = new List<ProductOrder>();
            request.CartItems.ForEach(cartItem =>
            {
                var product = lstProducts.FirstOrDefault(p => p.ProductId == cartItem.ProductId);
                if (product != null)
                {
                    var productOrder = new ProductOrder
                    {
                        ProductId = product.ProductId,
                        Quantity = cartItem.Quantity,
                        Price = product.PriceOutput,
                        OrderId = newOrder.OrderId
                    };
                    lstProductOrder.Add(productOrder);
                }
            });

            await _db.ProductOrders.AddRangeAsync(lstProductOrder);
            await _db.SaveChangesAsync();

            return new ResultCustomModel<int>
            {
                Code = 201,
                Data = newOrder.OrderId,
                Success = true,
                Message = "Thêm đơn hàng mới thành công"
            };
        }

        public async Task<ResultCustomModel<List<GetListOrderSPResult>>> GetListOrderAsync()
        {
            List<GetListOrderSPResult> rs = await _sp.GetListOrderSPAsync();
            return new ResultCustomModel<List<GetListOrderSPResult>>
            {
                Code = 200,
                Data = rs,
                Success = true,
                Message = "Tìm thấy danh sách sản phẩm"
            };
        }

        public async Task<ResultCustomModel<List<GetListOrderByCustomerIdSPResult>>> GetListOrderByCustomerIdAsync(int customerId)
        {
            List<GetListOrderByCustomerIdSPResult> orders = await _sp.GetListOrderByCustomerIdSPAsync(customerId);

            if (orders == null || !orders.Any())
            {
                return new ResultCustomModel<List<GetListOrderByCustomerIdSPResult>>
                {
                    Code = 404,
                    Success = false,
                    Message = "Không tìm thấy đơn hàng cho khách hàng này.",
                    Data = new List<GetListOrderByCustomerIdSPResult>()
                };
            }

            return new ResultCustomModel<List<GetListOrderByCustomerIdSPResult>>
            {
                Code = 200,
                Success = true,
                Message = "Lấy danh sách đơn hàng thành công.",
                Data = orders
            };
        }

        public async Task<ResultCustomModel<OrderReponse>> GetOrderByIdAsync(int orderId)
        {
            var order = await _db.Orders.FindAsync(orderId);

            if (order == null)
            {
                return new ResultCustomModel<OrderReponse>
                {
                    Code = 404,
                    Data = null,
                    Success = false,
                    Message = "Không tìm thấy đơn hàng với ID đã cho"
                };
            }

            var orderResponse = new OrderReponse
            {
                OrderId = order.OrderId,
                CustomerId = order.CustomerId,
                State = order.State,
                Note = order.Note,
                CreateOn = order.CreateOn,
                TotalAmount = order.TotalAmount,
            };

            return new ResultCustomModel<OrderReponse>
            {
                Code = 200,
                Data = orderResponse,
                Success = true,
                Message = "Lấy thông tin đơn hàng thành công"
            };
        }

        public async Task<ResultCustomModel<DetailOrderReponse>> GetListDetailOrderAsync(int orderId)
        {
            List<GetDetailProductOrderSPResult> rs = await _sp.GetDetailProductOrderSPAsync(orderId);

            Order order = _db.Orders.FirstOrDefault(x => x.OrderId == orderId);

            int customerId = order.CustomerId;

            Customer customer = _db.Customers.FirstOrDefault(x => x.CustomerId == customerId);

            DetailOrderReponse detailOrder = new DetailOrderReponse()
            {
                DetailProducts = rs,
                NameCustomer = customer.FullName,
                Address = customer.Address,
                StateId = order.State,
            };

            return new ResultCustomModel<DetailOrderReponse>
            {
                Code = 200,
                Data = detailOrder,
                Success = true,
                Message = "Lấy thông tin chi tiết sản phẩm trong đơn hàng thành công"
            };
        }

        public async Task<ResultCustomModel<bool>> ChangeStateOrderAsync(int orderId, short stateId)
        {
            Order order = await _db.Orders.FirstOrDefaultAsync(x => x.OrderId == orderId);
            if (order == null)
            {
                return new ResultCustomModel<bool>
                {
                    Code = 404,
                    Data = false,
                    Success = false,
                    Message = "Không tìm thấy đơn hàng"
                };
            }
            else
            {
                order.State = stateId;
                _db.Entry(order).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return new ResultCustomModel<bool>
                {
                    Code = 200,
                    Data = true,
                    Success = true,
                    Message = "Thay đổi trạng thái đơn hàng thành công"
                };
            }
        }

        public async Task<ResultCustomModel<List<RevenueResponse>>> GetRevenueLast7DaysAsync()
        {
            var startDate = DateTime.UtcNow.AddDays(-7);
            var endDate = DateTime.UtcNow;

            var revenues = await _db.Orders
                .Where(order => order.CreateOn >= startDate && order.CreateOn <= endDate)
                .GroupBy(order => order.CreateOn.Date)
                .Select(group => new RevenueResponse
                {
                    Date = group.Key,
                    TotalRevenue = group.Sum(order => order.TotalAmount)
                })
                .ToListAsync();

            for (int i = 0; i < 7; i++)
            {
                var date = DateTime.UtcNow.Date.AddDays(-i);
                if (!revenues.Any(r => r.Date == date))
                {
                    revenues.Add(new RevenueResponse
                    {
                        Date = date,
                        TotalRevenue = 0
                    });
                }
            }

            var sortedRevenues = revenues.OrderBy(r => r.Date).ToList();

            return new ResultCustomModel<List<RevenueResponse>>
            {
                Code = 200,
                Data = sortedRevenues,
                Success = true,
                Message = "Lấy doanh thu trong 7 ngày gần đây thành công"
            };
        }

        public async Task<ResultCustomModel<int>> GetTotalOrdersAsync()
        {
            try
            {
                int totalOrders = await _db.Orders.CountAsync();

                return new ResultCustomModel<int>
                {
                    Code = 200,
                    Data = totalOrders,
                    Success = true,
                    Message = "Tổng số đơn hàng "
                };
            }
            catch (Exception ex)
            {
                return new ResultCustomModel<int>
                {
                    Code = 500,
                    Data = 0,
                    Success = false,
                    Message = $"Lỗi khi tính tổng số đơn hàng: {ex.Message}"
                };
            }
        }

        public async Task<ResultCustomModel<bool>> CaptureOrderAsync(int orderId)
        {
            if (orderId <= 0)
            {
                return new ResultCustomModel<bool>
                {
                    Code = 400,
                    Data = false,
                    Success = false,
                    Message = "ID đơn hàng không hợp lệ"
                };
            }

            var order = await _db.Orders
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return new ResultCustomModel<bool>
                {
                    Code = 404,
                    Data = false,
                    Success = false,
                    Message = "Không tìm thấy đơn hàng"
                };
            }

            order.IsPaid = true;
            _db.Entry(order).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            // ✅ Gửi email xác nhận
            if (!string.IsNullOrEmpty(order.Customer?.Email))
            {
                try
                {
                    await _emailService.SendOrderConfirmationEmail(order.Customer.Email, order);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Lỗi khi gửi email xác nhận: " + ex.Message);
                    // Không fail nếu email lỗi, chỉ ghi log
                }
            }

            return new ResultCustomModel<bool>
            {
                Code = 200,
                Data = true,
                Success = true,
                Message = "Thanh toán đơn hàng thành công"
            };
        }

    }
}