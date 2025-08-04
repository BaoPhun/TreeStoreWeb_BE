using TreeStore.Models.CustomModels;
using TreeStore.Models.FavoriteModels;

namespace TreeStore.Services.FavoriteServices
{
    public interface IFavoriteServices
    {
        Task<ResultCustomModel<List<FavoriteProductResponse>>> GetFavoritesByCustomerAsync(int customerId);
        Task<ResultCustomModel<string>> AddFavoriteAsync(int customerId, int productId);
        Task<ResultCustomModel<string>> RemoveFavoriteAsync(int customerId, int productId);
    }
}
