namespace TreeStore.Models.Order
{
    public class OrderReponse
    {
        //public int UserId { get; set; }
        public int OrderId { get; set; }
        public int CustomerId { get; set; }

        public short State { get; set; }
        public string Note { get; set; }
        public DateTime CreateOn { get; set; }
        public decimal TotalAmount { get; set; }
        public string? PromotionCode { get; set; }
    }
   
}
