using Microsoft.EntityFrameworkCore;
using TreeStore.Models.CustomerModels;
using TreeStore.Models.CustomModels;
using TreeStore.Models.Entities;
using TreeStore.Services.Interfaces;

namespace TreeStore.Services
{
    public class ReviewServices : BaseServices, IReviewServices 
    {
        public ReviewServices(TreeStoreDBContext db, ITreeStoreDBContextProcedures sp) : base(db, sp)
        {
        }

        public async Task<ResultCustomModel<List<GetReviewInfoWithCustomerResult>>> ListCustomersAsync()
        {
            List<GetReviewInfoWithCustomerResult> rs = await _sp.GetReviewInfoWithCustomerAsync();
            return new ResultCustomModel<List<GetReviewInfoWithCustomerResult>>
            {
                Code = 200,
                Data = rs,
                Success = true,
                Message = "Tìm thấy danh sách sản phẩm"
            };
        }

        public async Task<ResultCustomModel<bool>> DeleteReview(int reviewId)
        {
            var review = await _db.Reviews.FirstOrDefaultAsync(x => x.ReviewId == reviewId);
            if (review == null)
            {
                return new ResultCustomModel<bool>
                {
                        
                    Code = 404,
                    Success = false,
                    Message = "Không tìm thấy đánh giá"
                };
            }
            _db.Reviews.Remove(review);
            await _db.SaveChangesAsync();

            return new ResultCustomModel<bool>
            {
                Code = 200,
                Data = true,
                Success = true,
                Message = "Xóa thành công "
            };
        }
       
        private ReviewResponse mapToReview(Review review)
        {

            return new ReviewResponse
            {
                ReviewId =review.ReviewId,
               CustomerId = review.CustomerId,
               ProductId = review.ProductId,
               Comment = review.Comment,


            };
        }

        public async Task<ResultCustomModel<bool>> CreateReview(ReviewRequest request)
        {
            // Tạo đối tượng Review mới từ request
            var newReview = new Review
            {
                //ReviewId = request.ReviewId,
                CustomerId = request.CustomerId,
                ProductId = request.ProductId,
                Comment = request.Comment,
                CreateOn = DateTime.Now,
                // Nếu bạn có thêm thuộc tính như Rating hoặc ReviewDate, có thể gán chúng ở đây
                // Rating = request.Rating,
                // ReviewDate = DateTime.Now
            };

            // Thêm đối tượng Review vào DbSet và lưu thay đổi vào database
            _db.Reviews.Add(newReview);
            await _db.SaveChangesAsync();

            // Trả về kết quả thành công (true)
            return new ResultCustomModel<bool>
            {
                Code = 200,
                Success = true,
                Message = "Tạo bình luận thành công",
                Data = true
            };
        }


        public async Task<ResultCustomModel<ReviewResponse>> GetReviewByIdAsync(int reviewId)
        {
            var review = await _db.Reviews.FirstOrDefaultAsync(x => x.ReviewId == reviewId);


            bool hasReview = review != null;

            ReviewResponse reviewResponse = new ReviewResponse();

            if (hasReview)
            {
                reviewResponse = mapToReview(review);
            }


            return new ResultCustomModel<ReviewResponse>
            {
                Code = hasReview ? 200 : 400,
                Data = reviewResponse,
                Success = hasReview,
                Message = hasReview ? "Lấy bình luận thành công" : "Lấy bình luận thất bại"
            };
        }

        public async Task<ResultCustomModel<List<ReviewResponse>>> GetReviewsByProductIdAsync(int productId)
        {
            // Lấy danh sách đánh giá kèm thông tin khách hàng
            var reviews = await (from review in _db.Reviews
                                 join customer in _db.Customers on review.CustomerId equals customer.CustomerId
                                 where review.ProductId == productId
                                 select new ReviewResponse
                                 {
                                     ReviewId = review.ReviewId,
                                     CustomerId = review.CustomerId,
                                     ProductId = review.ProductId,
                                     Comment = review.Comment,
                                     FullName = customer.FullName // Thêm FullName từ bảng Customer
                                 }).ToListAsync();

            return new ResultCustomModel<List<ReviewResponse>>
            {
                Code = 200,
                Data = reviews,
                Success = true,
                Message = "Lấy danh sách đánh giá thành công!"
            };
        }

        public async Task<ResultCustomModel<int>> GetTotalReviewsAsync()
        {
            try
            {
                int totalReviews = await _db.Reviews.CountAsync();

                return new ResultCustomModel<int>
                {
                    Code = 200,  // Trả về mã trạng thái thành công
                    Data = totalReviews,
                    Success = true,
                    Message = "Tổng số bình luận"
                };
            }
            catch (Exception ex)
            {
                return new ResultCustomModel<int>
                {
                    Code = 500,  // Trả về mã trạng thái lỗi nếu có lỗi
                    Data = 0,
                    Success = false,
                    Message = $"Lỗi khi tổng số bình luận: {ex.Message}"
                };
            }
        }

    }
}
