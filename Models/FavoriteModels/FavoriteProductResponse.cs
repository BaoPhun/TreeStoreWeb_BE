namespace TreeStore.Models.FavoriteModels
{
    public class FavoriteProductResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Img { get; set; }
        public decimal PriceOutput { get; set; }
        public bool IsActive { get; set; }
    }
}
