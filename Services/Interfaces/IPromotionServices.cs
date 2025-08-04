using TreeStore.Models.CustomModels;
using TreeStore.Models.Entities;
using TreeStore.Models.PromotionModels;

namespace TreeStore.Services.Interfaces
{
    public interface IPromotionServices
    {
        Task<ResultCustomModel<bool>> CreatePromotion(PromotionRequest request);

        Task ActivatePromotionAsync (string promotionCode);
        Task DeactivatePromotion(string promotionCode);
        Task<ResultCustomModel<bool>> ChangeActiveAsync(string promotionCode);
        Task<ResultCustomModel<PromotionResponse>> GetPromotionByIdAsync(string promotionCode);
        Task<ResultCustomModel<PromotionResponse>> GetPromotionByCodeAsync(string promotionCode);
        Task<ResultCustomModel<PromotionResponse>> CheckPromotionCodeAsync(string promotionCode, decimal totalAmount);
        Task<ResultCustomModel<PromotionResponse>> UpdatePromotion(PromotionRequest request);
        


        Task<ResultCustomModel<List<PromotionResponse>>> ListPromotion();
    }
}
