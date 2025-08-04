using Microsoft.AspNetCore.Mvc;
using TreeStore.Models.CustomModels;
using TreeStore.Models.FavoriteModels;
using TreeStore.Services.FavoriteServices;

namespace TreeStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteServices _favoriteService;

        public FavoritesController(IFavoriteServices favoriteService)
        {
            _favoriteService = favoriteService;
        }

        // Lấy danh sách sản phẩm yêu thích của khách hàng
        [HttpGet("{customerId}")]
        public async Task<ActionResult<ResultCustomModel<List<FavoriteProductResponse>>>> GetFavorites(int customerId)
        {
            var result = await _favoriteService.GetFavoritesByCustomerAsync(customerId);
            return Ok(result);
        }

        // Thêm sản phẩm vào yêu thích
        [HttpPost]
        public async Task<ActionResult<ResultCustomModel<string>>> AddFavorite([FromBody] FavoriteRequest request)
        {
            var result = await _favoriteService.AddFavoriteAsync(request.CustomerId, request.ProductId);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        // Xóa sản phẩm khỏi yêu thích
        [HttpDelete]
        public async Task<ActionResult<ResultCustomModel<string>>> RemoveFavorite([FromBody] FavoriteRequest request)
        {
            var result = await _favoriteService.RemoveFavoriteAsync(request.CustomerId, request.ProductId);
            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }
    }
}
