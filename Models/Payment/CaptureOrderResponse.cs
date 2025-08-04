namespace TreeStore.Models.Payment
{
    public class CaptureOrderResponse
    {
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
