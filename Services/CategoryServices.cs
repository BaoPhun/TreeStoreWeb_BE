using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using TreeStore.Models.Category;
using TreeStore.Models.CustomModels;
using TreeStore.Models.Entities;
using TreeStore.Services.Interfaces;

namespace TreeStore.Services
{
    public class CategoryService : BaseServices, ICategoryService
    {
        public CategoryService(TreeStoreDBContext db, ITreeStoreDBContextProcedures sp) : base(db, sp)
        {
        }
        // Xem tất cả các thể loại cây
        public async Task<ResultCustomModel<List<Category>>> GetAllCategoriesAsync()
        {
            List<Category> categories = await _db.Categories.ToListAsync();
            return new ResultCustomModel<List<Category>>
            {
                Code = 200,
                Data = categories,
                Success = true,
                Message = "Lấy danh sách thể loại thành công!"
            };
        }

        // Xem thể loại cây theo ID
        public async Task<ResultCustomModel<Category>> GetCategoryByIdAsync(int categoryId)
        {
            Category category = await _db.Categories.FindAsync(categoryId);
            if (category == null)
            {
                return new ResultCustomModel<Category>
                {
                    Code = 404,
                    Data = null,
                    Success = false,
                    Message = "Thể loại không tồn tại"
                };
            }

            return new ResultCustomModel<Category>
            {
                Code = 200,
                Data = category,
                Success = true,
                Message = "Lấy thông tin thể loại thành công"
            };
        }

       
        
        // Thay đổi trạng thái kích hoạt (ChangeActive) danh mục
        public async Task<ResultCustomModel<bool>> ChangeActiveAsync(int categoryId)
        {
            var category = await _db.Categories.FindAsync(categoryId);
            if (category == null)
            {
                return new ResultCustomModel<bool>
                {
                    Code = 404,
                    Data = false,
                    Success = false,
                    Message = "Thể loại không tồn tại"
                };
            }

            category.IsActive = !category.IsActive;
            _db.Entry(category).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return new ResultCustomModel<bool>
            {
                Code = 200,
                Data = true,
                Success = true,
                Message = "Cập nhật trạng thái thành công"
            };
        }

        // Sửa thể loại cây
        public async Task<ResultCustomModel<bool>> UpdateCategoryAsync(UpdateCategoryRequest category)
        {
            var existingCategory = await _db.Categories.FindAsync(category.CategoryId);
            if (existingCategory == null)
            {
                return new ResultCustomModel<bool>
                {
                    Code = 404,
                    Data = false,
                    Success = false,
                    Message = "Thể loại không tồn tại"
                };
            }

            existingCategory.Name = category.Name;
            //existingCategory.Slug = category.Slug;
            //existingCategory.Image = category.Image;
            //existingCategory.IsActive = category.IsActive;
            //existingCategory.TotalProduct = category.TotalProduct;
            // Cập nhật các thuộc tính khác nếu cần...

            await _db.SaveChangesAsync();

            return new ResultCustomModel<bool>
            {
                Code = 200,
                Data = true,
                Success = true,
                Message = "Cập nhật thể loại thành công"
            };
        }

        // Thêm thể loại cây mới
        public async Task<ResultCustomModel<bool>> CreateCategoryAsync(CreateCategoryRequest request)
        {
            var existingCategory = await _db.Categories.FirstOrDefaultAsync(c => c.Name == request.Name);
            if (existingCategory != null)
            {
                return new ResultCustomModel<bool>
                {
                    Code = 400,
                    Data = false,
                    Success = false,
                    Message = "Tên thể loại đã tồn tại"
                };
            }

            // Tạo một đối tượng Category mới từ request
            var category = new Category
            {
                Name = request.Name,
                Slug = GenerateSlug(request.Name) // Tự động tạo giá trị Slug
            };

            // Thêm danh mục vào cơ sở dữ liệu
            _db.Categories.Add(category);
            await _db.SaveChangesAsync();

            return new ResultCustomModel<bool>
            {
                Code = 201,
                Data = true,
                Success = true,
                Message = "Thêm thể loại mới thành công"
            };
        }

        // Phương thức tạo Slug từ Name
        private string GenerateSlug(string name)
        {
            return name
                .ToLower()
                .Replace(" ", "-") // Thay thế khoảng trắng bằng dấu '-'
                .Replace("'", ""); // Xóa dấu nháy đơn nếu có
        }

        public async Task<ResultCustomModel<List<Category>>> SearchCategoriesByNameAsync(string name)
        {
            var result = new ResultCustomModel<List<Category>>();

            if (string.IsNullOrWhiteSpace(name))
            {
                result.Success = false;
                result.Message = "Tên danh mục không được để trống.";
                return result;
            }

            var categories = await _db.Categories
                .Where(c => c.Name.Contains(name))
                .ToListAsync();

            result.Success = true;
            result.Data = categories;
            return result;
        }

    }
}
