namespace TreeStore.Models.ProductModels
{
    public class ProductResponseSale
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal PriceOutput { get; set; }
        public int SaleOff { get; set; }
        public decimal FinalPrice { get; set; }
        public string Img { get; set; }
    }
}
