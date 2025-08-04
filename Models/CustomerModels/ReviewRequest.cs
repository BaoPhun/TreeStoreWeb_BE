namespace TreeStore.Models.CustomerModels
{
    public class ReviewRequest
    {
        public int ReviewId { get; set; }
        public int CustomerId { get; set; }
        public int ProductId { get; set; }

        public string Comment { get; set; }
    }
}
