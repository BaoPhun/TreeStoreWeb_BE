using System.Collections.Generic;
using System.Threading.Tasks;
using TreeStore.Models.Category;
using TreeStore.Models.CustomModels;
using TreeStore.Models.Entities;

namespace TreeStore.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<ResultCustomModel<List<Category>>> GetAllCategoriesAsync();
        Task<ResultCustomModel<Category>> GetCategoryByIdAsync(int categoryId);
        Task<ResultCustomModel<bool>> UpdateCategoryAsync(UpdateCategoryRequest category);
        Task<ResultCustomModel<bool>> CreateCategoryAsync(CreateCategoryRequest request);
        Task<ResultCustomModel<bool>> ChangeActiveAsync(int categoryId);
        Task<ResultCustomModel<List<Category>>> SearchCategoriesByNameAsync(string name);

    }
}
