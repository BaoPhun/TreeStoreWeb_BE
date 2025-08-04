namespace TreeStore.Models.CustomerModels
{
    public class ReviewResponse
    {
        public int ReviewId { get; set; }   
        public int CustomerId { get; set; }

        public int ProductId { get; set; }

        public string Comment { get; set; }
        public string FullName { get; set; }

    }
}
