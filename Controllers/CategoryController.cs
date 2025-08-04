using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TreeStore.Models.Category;
using TreeStore.Models.CustomModels;
using TreeStore.Models.Entities;
using TreeStore.Services;
using TreeStore.Services.Interfaces;

namespace TreeStore.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // Xem tất cả thể loại cây
        //https://localhost:7072/api/Category/GetAll
        [HttpGet]
        public async Task<ResultCustomModel<List<Category>>> GetAll()
        {
            return await _categoryService.GetAllCategoriesAsync();
        }

        // Xem thể loại cây theo ID
        //https://localhost:7072/api/Category/GetById/1
        [HttpGet("{categoryId}")]
        public async Task<ResultCustomModel<Category>> GetById(int categoryId)
        {
            return await _categoryService.GetCategoryByIdAsync(categoryId);
        }


        [HttpPost]
        public async Task<ResultCustomModel<bool>> ChangeActiveCategory([FromQuery] int categoryId)
        {
            return await _categoryService.ChangeActiveAsync(categoryId);
        }


        // Sửa thể loại cây
        //https://localhost:7072/api/Category/Update
        [HttpPut]
        public async Task<ResultCustomModel<bool>> Update([FromBody] UpdateCategoryRequest category)
        {
            return await _categoryService.UpdateCategoryAsync(category);
        }
        //Thêm thể loại cây
        //https://localhost:7072/api/Category/Create
        [HttpPost]
        public async Task<ResultCustomModel<bool>> Create([FromBody] CreateCategoryRequest request)
        {
            return await _categoryService.CreateCategoryAsync(request);
        }
        [HttpGet]
        public async Task<ActionResult<ResultCustomModel<List<Category>>>> Search(string name)
        {
            var result = await _categoryService.SearchCategoriesByNameAsync(name);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

    }
}
