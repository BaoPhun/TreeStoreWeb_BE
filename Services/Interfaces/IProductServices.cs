using TreeStore.Models.Category;
using TreeStore.Models.CustomModels;
using TreeStore.Models.Entities;
using TreeStore.Models.LoginModels;
using TreeStore.Models.ProductModels;
using TreeStore.Models.UserModels;

namespace TreeStore.Controllers
{
    public interface IProductServices
    {
        Task<ResultCustomModel<bool>> CreateProductAsync(CreateProductRequest request);
        Task<ResultCustomModel<bool>> DeleteProductAsync(int productId);
        Task<ResultCustomModel<List<Product>>> GetAllProductAsync();
        Task<ResultCustomModel<List<GetListProductSPResult>>> ListProductAsync();
        Task<ResultCustomModel<ProductResponse>> GetProductByIdAsync(int productId);
        Task<ResultCustomModel<UpdateProductReponse>> UpdateProductAsync(UpdateProductRequest request);
        //Task<ResultCustomModel<bool>> UpdateProductAsync(Product product);
        Task<ResultCustomModel<bool>> ChangeActiveAsync(int productId);

        //Task<ResultCustomModel<List<GetListProductSPResult>>> SearchProductByNameAsync(string name);//tìm sản phẩm theo tên
        Task<ResultCustomModel<List<CreateCategoryRequest>>> GetCategorysAsync();
        Task<ResultCustomModel<List<GetListProductSPResult>>> SearchProductsAsync(string? productName, decimal? minPrice, decimal? maxPrice);

        Task<ResultCustomModel<int>> GetTotalProductsAsync();
        Task<ResultCustomModel<List<GetListProductSPResult>>> GetProductsByCategoryAsync(int categoryId);

        Task<ResultCustomModel<List<ProductResponseSale>>> GetSaleOffProductsAsync();
        Task<ResultCustomModel<List<GetListProductSPResult>>> SearchByPriceAsync(decimal? minPrice, decimal? maxPrice);
        Task<ResultCustomModel<List<GetListProductSPResult>>> GetTopSellingProductsAsync(int top = 5);


    }
}