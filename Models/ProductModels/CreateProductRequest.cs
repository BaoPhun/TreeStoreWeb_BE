using TreeStore.Models.Category;

namespace TreeStore.Models.ProductModels
{
    public class CreateProductRequest
    {
        public string Name { get; set; }
        public int PriceOutput { get; set; }
        public string Img { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }
    }

}
