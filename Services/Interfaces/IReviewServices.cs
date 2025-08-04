using TreeStore.Models.CustomModels;
using TreeStore.Models.Entities;
using TreeStore.Models.CustomerModels;
using TreeStore.Models.PromotionModels;

namespace TreeStore.Services.Interfaces
{
    public interface IReviewServices

    {
        Task<ResultCustomModel<bool>> CreateReview(ReviewRequest request);
        //Task<ResultCustomModel<List<ReviewResponse>>> ListReview();
        //Task<ResultCustomModel<List<GetReviewInfoWithCustomerResult>>> ListCustomersAsync();
        Task<ResultCustomModel<ReviewResponse>> GetReviewByIdAsync(int reviewId);
        Task<ResultCustomModel<bool>> DeleteReview(int reviewId);
        Task<ResultCustomModel<List<GetReviewInfoWithCustomerResult>>> ListCustomersAsync();


        Task<ResultCustomModel<List<ReviewResponse>>> GetReviewsByProductIdAsync(int  productId);
        Task<ResultCustomModel<int>> GetTotalReviewsAsync();

    }
}
