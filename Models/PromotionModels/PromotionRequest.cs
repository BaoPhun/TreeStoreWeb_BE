namespace TreeStore.Models.PromotionModels
{
    public class PromotionRequest
    {
      
        public string ProgramName {  get; set; } 

        public string PromotionCode { get; set; }
      
        public string? Description {  get; set; } 
        public DateTime CreationDate { get; set; } 
        public DateTime StartDate { get; set; } 
        public DateTime EndDate { get; set; }   
        public decimal DiscountAmount {  get; set; } 
        public decimal MinimumPurchaseAmount { get; set;}
        public int? UsageLimit { get; set; }

        public bool IsActive { get; set; }

        public int? OrderId { get; set; }

    }
}
