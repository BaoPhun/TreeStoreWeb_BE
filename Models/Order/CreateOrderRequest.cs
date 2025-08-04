    namespace TreeStore.Models.Order
    {
        public class CreateOrderRequest
        {
            public int CustomerId { get; set; }
            public string Note { get; set; }
            public int? PromotionId { get; set; } // Dùng khi chỉ có ID
            public string PromotionCode { get; set; } // Dùng khi có mã code

        public List<CartItem> CartItems { get; set; }
        }

        public class CartItem
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }
    }
