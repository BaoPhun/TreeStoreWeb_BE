using Microsoft.AspNetCore.Mvc;
using TreeStore.Models.CustomModels;
using TreeStore.Models.PromotionModels;
using TreeStore.Services;
using TreeStore.Services.Interfaces;

namespace TreeStore.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PromotionController : ControllerBase // Chỉnh sửa thành ControllerBase để phù hợp với API Controller
    {
        private readonly IPromotionServices _promotionServices;

        public PromotionController(IPromotionServices promotionServices)
        {
            _promotionServices = promotionServices;
        }

        [HttpGet]
        public async Task<ResultCustomModel<List<PromotionResponse>>> ListPromotion()
        {
            return await _promotionServices.ListPromotion();
        }

        [HttpGet("{promotionCode}")]
        public async Task<ResultCustomModel<PromotionResponse>> GetPromotionByIdAsync(string promotionCode)
        {
            return await _promotionServices.GetPromotionByIdAsync(promotionCode);
        }

        [HttpGet]
        public async Task<IActionResult> GetPromotionByCode([FromQuery] string promotionCode)
        {
            var result = await _promotionServices.GetPromotionByCodeAsync(promotionCode);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ResultCustomModel<PromotionResponse>> CheckPromotionCode([FromBody]  CheckPromotionModel request)
        {
            return await _promotionServices.CheckPromotionCodeAsync(request.PromotionCode, request.TotalAmount);
        }



        [HttpPost]
        public async Task<ResultCustomModel<bool>> Create([FromBody] PromotionRequest request)
        {
            return await _promotionServices.CreatePromotion(request);
        }

        [HttpPost]
        public async Task<IActionResult> DeactivatePromotion([FromBody] ActiveRequestPromotion request)
        {
            try
            {
                await _promotionServices.DeactivatePromotion(request.PromotionCode);
                return Ok(new ResultCustomModel<string>
                {
                    Code = 200,
                    Data = "Mã khuyến mãi đã bị vô hiệu hóa thành công",
                    Success = true,
                    Message = "Vô hiệu hóa mã khuyến mãi thành công."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResultCustomModel<string>
                {
                    Code = 400,
                    Data = null,
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<ResultCustomModel<string>> ActivatePromotionAsync([FromBody] string promotionCode)
        {
            try
            {
                await _promotionServices.ActivatePromotionAsync(promotionCode);
                return new ResultCustomModel<string>
                {
                    Code = 200,
                    Data = "Kích hoạt mã khuyến mãi thành công",
                    Success = true,
                    Message = "Kích hoạt mã khuyến mãi thành công"
                };
            }
            catch (Exception ex)
            {
                return new ResultCustomModel<string>
                {
                    Code = 400,
                    Data = null,
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        

        [HttpPost]
        public async Task<ResultCustomModel<bool>> ChangeActive([FromQuery] string promotionCode)
        {
            return await _promotionServices.ChangeActiveAsync(promotionCode);
        }

        [HttpPut]
        public async Task<ResultCustomModel<PromotionResponse>> UpdatePromotion([FromBody] PromotionRequest request)
        {
            var result = await _promotionServices.UpdatePromotion(request);
            return result; 
            
        }
    }
}
