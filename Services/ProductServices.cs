using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TreeStore.Controllers;
using TreeStore.Models.Category;
using TreeStore.Models.CustomerModels;
using TreeStore.Models.CustomModels;
using TreeStore.Models.Entities;
using TreeStore.Models.ProductModels;
using TreeStore.Models.UserModels;
using TreeStore.Utilities;

namespace TreeStore.Services
{
    public class ProductServices : BaseServices, IProductServices

    {
        public ProductServices(TreeStoreDBContext db, ITreeStoreDBContextProcedures sp) : base(db, sp)
        {
        }

        public async Task<ResultCustomModel<bool>> ChangeActiveAsync(int productId)
        {
            var product = await _db.Products.FirstOrDefaultAsync(x => x.ProductId == productId);
            bool hasProduct = product != null;
            bool isSuccess = false;
            if (hasProduct)
            {
                product.IsActive = !product.IsActive;
                _db.Entry(product).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                isSuccess = true;
            }
            else
            {
                isSuccess = false;
            }

            return new ResultCustomModel<bool>
            {
                Code = isSuccess ? 200 : 404,
                Data = isSuccess,
                Success = isSuccess,
                Message = isSuccess ? "Cập nhật trạng thái thành công" : "Cập nhật trạng thái thất bại"
            };
        }
        public async Task<ResultCustomModel<bool>> CreateProductAsync(CreateProductRequest request)
        {
            // Kiểm tra điều kiện đầu vào
            if (request == null || string.IsNullOrWhiteSpace(request.Name) || request.PriceOutput <= 0 || request.Quantity < 0)
            {
                return new ResultCustomModel<bool>
                {
                    Code = 400,
                    Data = false,
                    Success = false,
                    Message = "Dữ liệu đầu vào không hợp lệ"
                };
            }

            var newProduct = new Product
            {
                Name = request.Name,
                PriceOutput = request.PriceOutput,
                Img = request.Img,
                IsActive = true,
                Description = request.Description,
                Quantity = request.Quantity,
                CategoryId = request.CategoryId,
            };

            // Thêm sản phẩm mới vào database
            _db.Products.Add(newProduct);
            await _db.SaveChangesAsync();

            return new ResultCustomModel<bool>
            {
                Code = 201,
                Data = true,
                Success = true,
                Message = "Thêm thông tin cây mới thành công"
            };
        }

        public async Task<ResultCustomModel<List<CreateCategoryRequest>>> GetCategorysAsync()
        {
            var categories = await _db.Categories
         .Select(c => new CreateCategoryRequest
         {
             CategoryId = c.CategoryId,
             Name = c.Name
         }).ToListAsync();

            return new ResultCustomModel<List<CreateCategoryRequest>>
            {
                Code = 200,
                Data = categories,
                Success = true,
                Message = "Lấy danh sách loại thành công!"
            };
        }

        public async Task<ResultCustomModel<bool>> DeleteProductAsync(int productId)
        {
            var product = await _db.Products.FindAsync(productId);
            if (product == null)
            {
                return new ResultCustomModel<bool>
                {
                    Code = 404,
                    Data = false,
                    Success = false,
                    Message = "Thông tin cây không tồn tại"
                };
            }

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();

            return new ResultCustomModel<bool>
            {
                Code = 200,
                Data = true,
                Success = true,
                Message = "Xóa thông tin cây thành công"
            };
        }

        // Xem tất cả các  cây
        public async Task<ResultCustomModel<List<Product>>> GetAllProductAsync()
        {
            List<Product> product = await _db.Products.ToListAsync();
            return new ResultCustomModel<List<Product>>
            {
                Code = 200,
                Data = product,
                Success = true,
                Message = "Lấy danh sách cây thành công!"
            };
        }

        public async Task<ResultCustomModel<ProductResponse>> GetProductByIdAsync(int productId)
        {
            var product = await _db.Products.FirstOrDefaultAsync(x => x.ProductId == productId);
            bool hasProduct = product != null;

            ProductResponse productResponse = default;

            if (hasProduct)
            {
                productResponse = new ProductResponse()
                {
                    ProductId = product.ProductId,
                    Name = product.Name,
                    PriceInput = product.PriceInput,
                    PriceOutput = product.PriceOutput,
                    Img = product.Img,
                    
                    IsSell = product.IsSell,
                    IsActive = product.IsActive,
                    Description = product.Description,
                    ShortDescription = product.ShortDescription,
                    SaleOff = product.SaleOff,
                    CategoryId = product.CategoryId,
                    IsBanner = product.IsBanner,
                    IsFeather = product.IsFeather,
                    IsSpecialOffer = product.IsSpecialOffer,
                    IsPopular = product.IsPopular,
                    BannerImg = product.BannerImg,
                    Slug = product.Slug,
                    Img2 = product.Img2,
                    Img3 = product.Img3,
                    Quantity = product.Quantity
                };

            }

            return new ResultCustomModel<ProductResponse>
            {
                Code = hasProduct ? 200 : 404,
                Data = productResponse,
                Success = hasProduct,
                Message = hasProduct ? "Lấy thông tin sản phẩm thành công" : "Sản phẩm không tồn tại"
            };
        }

        public async Task<ResultCustomModel<UpdateProductReponse>> UpdateProductAsync(UpdateProductRequest request)
        {
            // Kiểm tra điều kiện đầu vào
            if (request == null || request.ProductId <= 0 || string.IsNullOrWhiteSpace(request.Name) || request.PriceOutput <= 0 || request.Quantity < 0)
            {
                return new ResultCustomModel<UpdateProductReponse>
                {
                    Code = 400,
                    Data = null,
                    Success = false,
                    Message = "Dữ liệu đầu vào không hợp lệ"
                };
            }

            var existingProduct = await _db.Products.FindAsync(request.ProductId);
            if (existingProduct == null)
            {
                return new ResultCustomModel<UpdateProductReponse>
                {
                    Code = 404,
                    Data = null,
                    Success = false,
                    Message = "Thông tin cây không tồn tại"
                };
            }

            // Cập nhật các thuộc tính của sản phẩm
            existingProduct.Name = request.Name;
            existingProduct.PriceOutput = request.PriceOutput;
            existingProduct.Img = request.Img;
            existingProduct.Description = request.Description;
            existingProduct.Quantity = request.Quantity;
            existingProduct.CategoryId = request.CategoryId;

            // Lưu thay đổi vào cơ sở dữ liệu
            _db.Products.Update(existingProduct);
            await _db.SaveChangesAsync();

            var updateProductPeponse = new UpdateProductReponse
            {
                Name = existingProduct.Name,
                PriceOutput = existingProduct.PriceOutput,
                Img = existingProduct.Img,
                Description = existingProduct.Description,
                Quantity = existingProduct.Quantity,
                CategoryId = existingProduct.CategoryId
            };
            return new ResultCustomModel<UpdateProductReponse>
            {
                Code = 200,
                Data = updateProductPeponse,
                Success = true,
                Message = "Cập nhật thông tin cây thành công"
            };
        }



        // Tìm kiếm sản phẩm theo tên
        public async Task<ResultCustomModel<List<GetListProductSPResult>>> SearchProductByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new ResultCustomModel<List<GetListProductSPResult>>
                {
                    Code = 400,
                    Data = null,
                    Success = false,
                    Message = "Tên sản phẩm không được để trống"
                };
            }

            var products = await _db.Products
                .Where(p => p.Name.Contains(name)) // Tìm kiếm theo thuộc tính Name
                .Select(p => new GetListProductSPResult
                {
                    ProductId = p.ProductId,
                    ProductName = p.Name, // Ánh xạ từ Name sang ProductName
                    Description = p.Description,
                    PriceOutput = p.PriceOutput,
                    IsActive = p.IsActive,
                    Quantity = p.Quantity,
                    CategoryId = p.CategoryId,
                    Img = p.Img,
                    Img2 = p.Img2,
                    Img3 = p.Img3,
                    CategoryName = p.Category != null ? p.Category.Name : null // Kiểm tra Category nếu có
                })
                .ToListAsync();

            if (!products.Any()) // Kiểm tra nếu danh sách rỗng
            {
                return new ResultCustomModel<List<GetListProductSPResult>>
                {
                    Code = 404,
                    Data = null,
                    Success = false,
                    Message = "Không tìm thấy sản phẩm"
                };
            }

            return new ResultCustomModel<List<GetListProductSPResult>>
            {
                Code = 200,
                Data = products,
                Success = true,
                Message = "Tìm thấy sản phẩm"
            };
        }

        // Tìm kiếm sản phẩm theo tên và bộ lọc giá
        public async Task<ResultCustomModel<List<GetListProductSPResult>>> SearchProductsAsync(string productName, decimal? minPrice, decimal? maxPrice)
        {
            // Bắt đầu truy vấn sản phẩm từ database
            var query = _db.Products.AsQueryable();

            // Lọc theo tên sản phẩm nếu được cung cấp
            if (!string.IsNullOrWhiteSpace(productName))
            {
                query = query.Where(p => p.Name.Contains(productName));
            }

            // Lọc theo giá tối thiểu nếu được cung cấp
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.PriceOutput >= minPrice.Value);
            }

            // Lọc theo giá tối đa nếu được cung cấp
            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.PriceOutput <= maxPrice.Value);
            }

            // Ánh xạ dữ liệu và lấy kết quả
            var products = await query
                .Select(p => new GetListProductSPResult
                {
                    ProductId = p.ProductId,
                    ProductName = p.Name,
                    Description = p.Description,
                    PriceOutput = p.PriceOutput,
                    IsActive = p.IsActive,
                    Quantity = p.Quantity,
                    CategoryId = p.CategoryId,
                    Img = p.Img,
                    Img2 = p.Img2,
                    Img3 = p.Img3,
                    CategoryName = p.Category != null ? p.Category.Name : null
                })
                .ToListAsync();

            if (!products.Any()) // Kiểm tra nếu không tìm thấy sản phẩm
            {
                return new ResultCustomModel<List<GetListProductSPResult>>
                {
                    Code = 404,
                    Data = null,
                    Success = false,
                    Message = "Không tìm thấy sản phẩm"
                };
            }

            return new ResultCustomModel<List<GetListProductSPResult>>
            {
                Code = 200,
                Data = products,
                Success = true,
                Message = "Tìm thấy sản phẩm"
            };
        }



        public async Task<ResultCustomModel<List<GetListProductSPResult>>> ListProductAsync()
        {
            List<GetListProductSPResult> rs = await _sp.GetListProductSPAsync();
            return new ResultCustomModel<List<GetListProductSPResult>>
            {
                Code = 200,
                Data = rs,
                Success = true,
                Message = "Tìm thấy danh sách sản phẩm"
            };
        }

        public async Task<ResultCustomModel<int>> GetTotalProductsAsync()
        {
            try
            {
                int totalProducts = await _db.Products.CountAsync();

                return new ResultCustomModel<int>
                {
                    Code = 200,  // Trả về mã trạng thái thành công
                    Data = totalProducts,
                    Success = true,
                    Message = "Tổng số sản phẩm trong kho"
                };
            }
            catch (Exception ex)
            {
                return new ResultCustomModel<int>
                {
                    Code = 500,  // Trả về mã trạng thái lỗi nếu có lỗi
                    Data = 0,
                    Success = false,
                    Message = $"Lỗi khi tính sản phẩm đơn hàng: {ex.Message}"
                };
            }
        }

        public async Task<ResultCustomModel<List<ProductResponseSale>>> GetSaleOffProductsAsync()
        {
            var saleOffProducts = await _db.Products
                .Where(p => p.SaleOff > 0) // Lọc các sản phẩm có giảm giá
                .Select(p => new ProductResponseSale
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    PriceOutput = p.PriceOutput,
                    SaleOff = p.SaleOff,
                    FinalPrice = p.PriceOutput * (1 - p.SaleOff / 100.0m), // Tính giá sau giảm
                    Img = p.Img
                })
                .OrderByDescending(p => p.SaleOff) // Sắp xếp theo mức giảm giá
                .ToListAsync();

            return new ResultCustomModel<List<ProductResponseSale>>
            {
                Code = 200,
                Success = true,
                Data = saleOffProducts,
                Message = "Lấy danh sách sản phẩm giảm giá thành công"
            };
        }

        public async Task<ResultCustomModel<List<GetListProductSPResult>>> GetProductsByCategoryAsync(int categoryId)
        {
            var products = await _db.Products
                .Where(p => p.CategoryId == categoryId && p.IsActive == true) // Lọc theo CategoryId và sản phẩm đang hoạt động
                .Select(p => new GetListProductSPResult
                {
                    ProductId = p.ProductId,
                    ProductName = p.Name,
                    Description = p.Description,
                    PriceOutput = p.PriceOutput,
                    IsActive = p.IsActive,
                    Quantity = p.Quantity,
                    CategoryId = p.CategoryId,
                    Img = p.Img,
                    CategoryName = _db.Categories
                        .Where(c => c.CategoryId == p.CategoryId)
                        .Select(c => c.Name)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return new ResultCustomModel<List<GetListProductSPResult>>
            {
                Code = 200,
                Success = true,
                Data = products,
                Message = "Lấy danh sách sản phẩm theo danh mục thành công"
            };
        }
        public async Task<ResultCustomModel<List<GetListProductSPResult>>> SearchByPriceAsync(decimal? minPrice, decimal? maxPrice)
        {
            var query = _db.Products.AsQueryable();

            if (minPrice.HasValue)
                query = query.Where(p => p.PriceOutput >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.PriceOutput <= maxPrice.Value);

            var products = await query
                .Select(p => new GetListProductSPResult
                {
                    ProductId = p.ProductId,
                    ProductName = p.Name,
                    Description = p.Description,
                    PriceOutput = p.PriceOutput,
                    IsActive = p.IsActive,
                    Quantity = p.Quantity,
                    CategoryId = p.CategoryId,
                    Img = p.Img,
                    Img2 = p.Img2,
                    Img3 = p.Img3,
                    CategoryName = p.Category != null ? p.Category.Name : null
                })
                .ToListAsync();

            if (!products.Any())
            {
                return new ResultCustomModel<List<GetListProductSPResult>>
                {
                    Code = 404,
                    Data = null,
                    Success = false,
                    Message = "Không tìm thấy sản phẩm phù hợp với mức giá đã chọn"
                };
            }

            return new ResultCustomModel<List<GetListProductSPResult>>
            {
                Code = 200,
                Data = products,
                Success = true,
                Message = "Tìm thấy sản phẩm theo mức giá"
            };
        }

    }
}
