using Microsoft.AspNetCore.Mvc;
using TreeStore.Models.Category;
using TreeStore.Models.CustomModels;
using TreeStore.Models.Entities;
using TreeStore.Models.LoginModels;
using TreeStore.Models.ProductModels;
using TreeStore.Models.UserModels;
using TreeStore.Services;

namespace TreeStore.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly IProductServices _productsService;

        public ProductController(IProductServices productService)
        {
            _productsService = productService;
        }

       
        [HttpGet]
        public async Task<ResultCustomModel<List<Product>>> GetAll()
        {
            return await _productsService.GetAllProductAsync();
        }

        [HttpGet]
        public async Task<ResultCustomModel<List<GetListProductSPResult>>> ListProduct()
        {
            return await _productsService.ListProductAsync();
        }
        [HttpGet]
        public async Task<ResultCustomModel<ProductResponse>> GetProductByIdAsync([FromQuery] int productId)
        {
            return await _productsService.GetProductByIdAsync(productId);
        }
      
        [HttpDelete]
        public async Task<ResultCustomModel<bool>> Delete(int productId)
        {
            return await _productsService.DeleteProductAsync(productId);
        }

       
        [HttpPut]
        public async Task<ResultCustomModel<UpdateProductReponse>> Update([FromBody] UpdateProductRequest request)
        {
            return await _productsService.UpdateProductAsync(request);
        }

        [HttpGet]
        public async Task<ResultCustomModel<List<CreateCategoryRequest>>> GetCategorys()
        {
            return await _productsService.GetCategorysAsync();
        }
        [HttpPost]
        public async Task<ResultCustomModel<bool>> Create([FromBody] CreateProductRequest request)
        {   
            return await _productsService.CreateProductAsync(request);
        }
        // Tìm kiếm sản phẩm theo tên
        //[HttpGet]
        //public async Task<ResultCustomModel<List<GetListProductSPResult>>> SearchByName(string name)
        //{
        //    return await _productsService.SearchProductByNameAsync(name);
        //}

        [HttpGet]
        public async Task<ActionResult<ResultCustomModel<List<GetListProductSPResult>>>> SearchProducts(
                [FromQuery] string? productName = null,
                [FromQuery] decimal? minPrice = null,
                [FromQuery] decimal? maxPrice = null)
        {
            if (minPrice.HasValue && minPrice < 0)
            {
                return BadRequest(new { error = "minPrice phải lớn hơn hoặc bằng 0" });
            }
            if (maxPrice.HasValue && maxPrice < 0)
            {
                return BadRequest(new { error = "maxPrice phải lớn hơn hoặc bằng 0" });
            }

            var result = await _productsService.SearchProductsAsync(productName, minPrice, maxPrice);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<ResultCustomModel<bool>> ChangeActive([FromQuery] int productId)
        {
            return await _productsService.ChangeActiveAsync(productId);
        }

        [HttpGet("total-products")]
        public async Task<ResultCustomModel<int>> GetTotalProducts()
        {
            // Gọi service để tính tổng số đơn hàng
            var result = await _productsService.GetTotalProductsAsync();

            // Trả về kết quả (response) cho client
            return result;
        }

        [HttpGet]
        public async Task<ResultCustomModel<List<ProductResponseSale>>> GetSaleOffProducts()
        {
            return await _productsService.GetSaleOffProductsAsync();
        }
        [HttpGet]
        public async Task<ResultCustomModel<List<GetListProductSPResult>>> GetProductsByCategory([FromQuery] int categoryId)
        {
            return await _productsService.GetProductsByCategoryAsync(categoryId);
        }
        [HttpGet("search-by-price")]
        public async Task<ActionResult> SearchByPrice(decimal? minPrice, decimal? maxPrice)
        {
            var result = await _productsService.SearchByPriceAsync(minPrice, maxPrice);
            return Ok(result);
        }
    }
}
