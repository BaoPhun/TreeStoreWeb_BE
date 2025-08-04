namespace TreeStore.Models.ProductModels
{
    public class UpdateProductReponse
    {
        public string Name { get; set; }
        public int PriceOutput { get; set; }
        public string Img { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }
        public List<int> ListCategorys { get; set; }

    }
}
