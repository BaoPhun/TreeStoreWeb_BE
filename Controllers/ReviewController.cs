using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TreeStore.Models.CustomerModels;
using TreeStore.Models.CustomModels;
using TreeStore.Models.Entities;
using TreeStore.Models.LoginModels;
using TreeStore.Services;
using TreeStore.Services.Interfaces;


namespace TreeStore.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReviewController : Controller 
    {
        private readonly IReviewServices _reviewServices;

        public ReviewController(IReviewServices reviewServices)
        {
            _reviewServices = reviewServices;
        }

        //[HttpGet]
        //public async Task<ResultCustomModel<List<ReviewResponse>>> ListReview()
        //{
        //    return await _reviewServices.ListReview();
        //}
        [HttpGet]
        public async Task<ResultCustomModel<List<GetReviewInfoWithCustomerResult>>> ListCustomers()
        {
            return await _reviewServices.ListCustomersAsync();
        }
        [HttpGet("{GetId}")]
        public async Task<ResultCustomModel<ReviewResponse>> GetById(int reviewId)
        {
            return await _reviewServices.GetReviewByIdAsync(reviewId);
        }
        [HttpDelete("{GetId}")]
        public async Task<ResultCustomModel<bool>> DeleteReview(int reviewId)
        {
            return await _reviewServices.DeleteReview(reviewId);
        }
        [HttpPost]
        public async Task<ResultCustomModel<bool>> CreateReview([FromBody] ReviewRequest request)
        {
            return await _reviewServices.CreateReview(request);
        }
        // GET: api/review/product/{productId}
        [HttpGet("{productId}")]
        public async Task<ResultCustomModel<List<ReviewResponse>>> GetReviewsByProductIdAsync(int productId)
        {
            return await _reviewServices.GetReviewsByProductIdAsync (productId);
        }
        [HttpGet("total-reviews")]
        public async Task<ResultCustomModel<int>> GetTotalReviews()
        {
            // Gọi service để tính tổng số đơn hàng
            var result = await _reviewServices.GetTotalReviewsAsync();

            // Trả về kết quả (response) cho client
            return result;
        }
    }
}
