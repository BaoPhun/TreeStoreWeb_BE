using Microsoft.EntityFrameworkCore;
using TreeStore.Models.CustomModels;
using TreeStore.Models.Entities;
using TreeStore.Models.FavoriteModels;
using TreeStore.Services.FavoriteServices;

namespace TreeStore.Services
{
    public class FavoriteService : BaseServices, IFavoriteServices
    {
        public FavoriteService(TreeStoreDBContext db, ITreeStoreDBContextProcedures sp) : base(db, sp)
        {
        }

        // Lấy danh sách sản phẩm yêu thích theo customerId
        public async Task<ResultCustomModel<List<FavoriteProductResponse>>> GetFavoritesByCustomerAsync(int customerId)
        {
            var favorites = await _db.Favorites
                .Include(f => f.Product)
                .Where(f => f.CustomerId == customerId)
                .Select(f => new FavoriteProductResponse
                {
                    ProductId = f.Product.ProductId,
                    ProductName = f.Product.Name,
                    Img = f.Product.Img,
                    PriceOutput = f.Product.PriceOutput,
                })
                .ToListAsync();

            return new ResultCustomModel<List<FavoriteProductResponse>>
            {
                Code = 200,
                Data = favorites,
                Success = true,
                Message = "Lấy danh sách sản phẩm yêu thích thành công!"
            };
        }

        // Thêm sản phẩm vào danh sách yêu thích
        public async Task<ResultCustomModel<string>> AddFavoriteAsync(int customerId, int productId)
        {
            var exists = await _db.Favorites.AnyAsync(f => f.CustomerId == customerId && f.ProductId == productId);
            if (exists)
            {
                return new ResultCustomModel<string>
                {
                    Code = 400,
                    Data = null,
                    Success = false,
                    Message = "Sản phẩm đã tồn tại trong danh sách yêu thích!"
                };
            }

            var favorite = new Favorite
            {
                CustomerId = customerId,
                ProductId = productId,
                CreateOn = DateTime.UtcNow
            };

            _db.Favorites.Add(favorite);
            await _db.SaveChangesAsync();

            return new ResultCustomModel<string>
            {
                Code = 200,
                Data = "Đã thêm vào danh sách yêu thích!",
                Success = true,
                Message = "Thêm thành công!"
            };
        }

        // Xoá sản phẩm khỏi danh sách yêu thích
        public async Task<ResultCustomModel<string>> RemoveFavoriteAsync(int customerId, int productId)
        {
            var favorite = await _db.Favorites.FirstOrDefaultAsync(f => f.CustomerId == customerId && f.ProductId == productId);
            if (favorite == null)
            {
                return new ResultCustomModel<string>
                {
                    Code = 404,
                    Data = null,
                    Success = false,
                    Message = "Không tìm thấy sản phẩm yêu thích!"
                };
            }

            _db.Favorites.Remove(favorite);
            await _db.SaveChangesAsync();

            return new ResultCustomModel<string>
            {
                Code = 200,
                Data = "Đã xoá khỏi danh sách yêu thích!",
                Success = true,
                Message = "Xoá thành công!"
            };
        }
    }
}
