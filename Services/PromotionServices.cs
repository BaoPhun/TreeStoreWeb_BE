using Microsoft.EntityFrameworkCore;
using TreeStore.Models.CustomerModels;
using TreeStore.Models.CustomModels;
using TreeStore.Models.Entities;
using TreeStore.Models.PromotionModels;
using TreeStore.Services.Interfaces;

namespace TreeStore.Services
{
    public class PromotionServices : BaseServices, IPromotionServices
    {
        public PromotionServices(TreeStoreDBContext db, ITreeStoreDBContextProcedures sp) : base(db, sp)
        {
        }

        public async Task<ResultCustomModel<bool>> CreatePromotion(PromotionRequest request)
        {
            // Kiểm tra mã khuyến mãi đã tồn tại chưa
            bool isPromotion = await _db.Promotions.AnyAsync(x => x.PromotionCode.Equals(request.PromotionCode));
            if (isPromotion)
            {
                return new ResultCustomModel<bool>
                {
                    Code = 404,
                    Success = false,
                    Message = "Mã khuyến mãi đã tồn tại",
                    Data = false
                };
            }

            // Tạo đối tượng Promotion mới từ request
            var newPromotion = new Promotion
            {
                ProgramName = request.ProgramName,
                PromotionCode = request.PromotionCode,
                Description = request.Description,
                CreationDate = DateTime.Now,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                DiscountAmount = request.DiscountAmount,
                MinimumPurchaseAmount = request.MinimumPurchaseAmount,
                UsageLimit = request.UsageLimit,
                IsActive = request.IsActive, // Đặt trạng thái mặc định cho khuyến mãi là hoạt động\
                //OrderId = request.OrderId // Gán OrderId từ request

            };

            // Thêm đối tượng Promotion vào DbSet và lưu thay đổi vào database
            _db.Promotions.Add(newPromotion);
            await _db.SaveChangesAsync();

            // Trả về kết quả thành công
            return new ResultCustomModel<bool>
            {
                Code = 200,
                Success = true,
                Message = "Tạo khuyến mãi thành công",
                Data = true
            };
        }



        public async Task ActivatePromotionAsync(string promotionCode)
        {
            var promotion = await _db.Promotions.FirstOrDefaultAsync(x => x.PromotionCode == promotionCode);
            if (promotion == null)
            {
                throw new Exception("Mã khuyến mãi không tồn tại");
            }

            promotion.IsActive = true;
            _db.Promotions.Update(promotion);
            await _db.SaveChangesAsync();
        }

        public async Task DeactivatePromotion(string promotionCode)
        {
            var promotion = await _db.Promotions.FirstOrDefaultAsync(x => x.PromotionCode == promotionCode);
            if (promotion == null)
            {
                throw new Exception("Mã khuyến mãi không tồn tại");
            }

            promotion.IsActive = false;
            _db.Promotions.Update(promotion);
            await _db.SaveChangesAsync();
        }

        public async Task<ResultCustomModel<bool>> ChangeActiveAsync(string promotionCode)
        {
            var promotion = await _db.Promotions.FirstOrDefaultAsync(x => x.PromotionCode == promotionCode);
            if (promotion == null)
            {
                return new ResultCustomModel<bool>
                {
                    Code = 404,
                    Success = false,
                    Message = "Mã khuyến mãi không tồn tại",
                    Data = false
                };
            }

            promotion.IsActive = !promotion.IsActive;
            _db.Entry(promotion).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return new ResultCustomModel<bool>
            {
                Code = 200,
                Data = true,
                Success = true,
                Message = "Cập nhật trạng thái khuyến mãi thành công"
            };
        }

        public async Task<ResultCustomModel<PromotionResponse>> GetPromotionByIdAsync(string promotionCode)
        {
            var promotion = await _db.Promotions.FirstOrDefaultAsync(x => x.PromotionCode == promotionCode);

            bool hasPromotion = promotion != null;

            PromotionResponse promotionResponse = new PromotionResponse();

            if (hasPromotion)
            {
                promotionResponse = mapToPromotion(promotion);
            }

            return new ResultCustomModel<PromotionResponse>
            {
                Code = hasPromotion ? 200 : 404,
                Data = promotionResponse,
                Success = hasPromotion,
                Message = hasPromotion ? "Lấy thông tin khuyến mãi thành công" : "Khuyến mãi không tồn tại"
            };
        }
        public async Task<ResultCustomModel<PromotionResponse>> GetPromotionByCodeAsync(string promotionCode)
        {
            var promotion = await _db.Promotions.FirstOrDefaultAsync(x => x.PromotionCode == promotionCode);

            bool hasPromotion = promotion != null;
            PromotionResponse promotionResponse = new PromotionResponse();

            if (hasPromotion)
            {
                promotionResponse = mapToPromotion(promotion);
            }

            return new ResultCustomModel<PromotionResponse>
            {
                Code = hasPromotion ? 200 : 404,
                Data = promotionResponse,
                Success = hasPromotion,
                Message = hasPromotion ? "Lấy thông tin khuyến mãi thành công" : "Khuyến mãi không tồn tại"
            };
        }

        //Kiểm tra PromotionCode
        public async Task<ResultCustomModel<PromotionResponse>> CheckPromotionCodeAsync(string promotionCode, decimal totalAmount)
        {
            // Kiểm tra mã khuyến mãi trong cơ sở dữ liệu
            var promotion = await _db.Promotions
                                      .FirstOrDefaultAsync(x => x.PromotionCode == promotionCode && x.IsActive);

            if (promotion == null)
            {
                return new ResultCustomModel<PromotionResponse>
                {
                    Code = 404,
                    Success = false,
                    Message = "Mã khuyến mãi không tồn tại hoặc không còn hiệu lực",
                    Data = null
                };
            }

            // Kiểm tra ngày hợp lệ của mã khuyến mãi
            var currentDate = DateTime.UtcNow;
            if (currentDate < promotion.StartDate || currentDate > promotion.EndDate)
            {
                return new ResultCustomModel<PromotionResponse>
                {
                    Code = 400,
                    Success = false,
                    Message = "Mã khuyến mãi không áp dụng trong thời gian này",
                    Data = null
                };
            }

            // Kiểm tra điều kiện chi tiêu tối thiểu
            if (totalAmount < promotion.MinimumPurchaseAmount)
            {
                return new ResultCustomModel<PromotionResponse>
                {
                    Code = 400,
                    Success = false,
                    Message = $"Mã khuyến mãi chỉ áp dụng cho đơn hàng từ {promotion.MinimumPurchaseAmount:C}",
                    Data = null
                };
            }

            // Tính toán số tiền giảm
            decimal discount = promotion.DiscountAmount;
            decimal finalAmount = totalAmount - discount;

            // Nếu số tiền giảm quá lớn, có thể giảm tối đa là tổng giá trị đơn hàng
            if (finalAmount < 0)
            {
                finalAmount = 0;
            }

            // Tạo đối tượng PromotionResponse
            var promotionResponse = new PromotionResponse
            {
                ProgramName = promotion.ProgramName,
                PromotionCode = promotion.PromotionCode,
                Description = promotion.Description,
                CreationDate = promotion.CreationDate,
                StartDate = promotion.StartDate,
                EndDate = promotion.EndDate,
                DiscountAmount = promotion.DiscountAmount,
                MinimumPurchaseAmount = promotion.MinimumPurchaseAmount,
                UsageLimit = promotion.UsageLimit,
                IsActive = promotion.IsActive,
                FinalAmount = finalAmount  // Trả về giá trị sau khi giảm
            };

            return new ResultCustomModel<PromotionResponse>
            {
                Code = 200,
                Success = true,
                Message = "Mã khuyến mãi hợp lệ",
                Data = promotionResponse
            };
        }




        public async Task<ResultCustomModel<List<PromotionResponse>>> ListPromotion()
        {
            // Lấy danh sách khuyến mãi từ cơ sở dữ liệu
            var promotions = await _db.Promotions.ToListAsync();

            // Chuyển đổi danh sách khuyến mãi thành danh sách PromotionResponse
            var promotionResponses = promotions.Select(p => mapToPromotion(p)).ToList();

            return new ResultCustomModel<List<PromotionResponse>>
            {
                Code = 200, // Mã trạng thái thành công
                Data = promotionResponses, // Danh sách khuyến mãi đã chuyển đổi
                Success = true, // Thao tác thành công
                Message = "Thành công!" // Thông báo
            };
        }

        public async Task<ResultCustomModel<PromotionResponse>> UpdatePromotion(PromotionRequest request)
        {
            if (request == null)
            {
                return new ResultCustomModel<PromotionResponse>
                {
                    Code = 400,
                    Success = false,
                    Message = "Yêu cầu không hợp lệ"
                };
            }

            if (request.StartDate >= request.EndDate)
            {
                return new ResultCustomModel<PromotionResponse>
                {
                    Code = 400,
                    Success = false,
                    Message = "Ngày bắt đầu phải nhỏ hơn ngày kết thúc"
                };
            }

            var promotion = await _db.Promotions.FirstOrDefaultAsync(p => p.PromotionCode == request.PromotionCode);

            if (promotion == null)
            {
                return new ResultCustomModel<PromotionResponse>
                {
                    Code = 404,
                    Success = false,
                    Message = "Khuyến mãi không tồn tại"
                };
            }

            // Cập nhật thông tin khuyến mãi
            promotion.ProgramName = request.ProgramName;
            promotion.Description = request.Description;
            promotion.PromotionCode= request.PromotionCode; 
            promotion.CreationDate = request.CreationDate;
            promotion.EndDate = request.EndDate;
            promotion.StartDate = request.StartDate;
            promotion.DiscountAmount = request.DiscountAmount;
            promotion.UsageLimit = request.UsageLimit;
            promotion.IsActive = request.IsActive;
            //promotion.OrderId = request.OrderId;


            _db.Promotions.Update(promotion);
            await _db.SaveChangesAsync();

            var promotionResponse = mapToPromotion(promotion);

            return new ResultCustomModel<PromotionResponse>
            {
                Code = 200,
                Data = promotionResponse,
                Success = true,
                Message = "Cập nhật thành công"
            };
        }

        private PromotionResponse mapToPromotion(Promotion promotion)
        {
            return new PromotionResponse
            {

                PromotionCode = promotion.PromotionCode,
                ProgramName = promotion.ProgramName,
                Description = promotion.Description,
                CreationDate = promotion.CreationDate,
                StartDate = promotion.StartDate,
                EndDate = promotion.EndDate,
                DiscountAmount = promotion.DiscountAmount,
                MinimumPurchaseAmount = promotion.MinimumPurchaseAmount,
                UsageLimit = promotion.UsageLimit,
                IsActive = promotion.IsActive,
                //OrderId = promotion.OrderId,
            };
        }
    }
}
