using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TreeStore;
using TreeStore.Models.Entities;
using TreeStore.Models.CustomModels;

namespace TreeStore.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        private readonly TreeStoreDBContext _dbContext;

        public AuthController(TreeStoreDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> GoogleSignIn([FromBody] GoogleSignInRequest request)
        {
            if (string.IsNullOrEmpty(request.IdToken))
            {
                return BadRequest(new ResultCustomModel<object>
                {
                    Code = 400,
                    Success = false,
                    Message = "IdToken không được để trống."
                });
            }

            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>()
                    {
                        "122754317810-s8g8tikvthd720j11eefl500s3j5lllo.apps.googleusercontent.com" // Thay clientId của bạn vào đây
                    }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);

                if (payload == null)
                {
                    return Unauthorized(new ResultCustomModel<object>
                    {
                        Code = 401,
                        Success = false,
                        Message = "Token Google không hợp lệ."
                    });
                }

                // Tìm khách hàng theo email
                var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Email == payload.Email);

                if (customer == null)
                {
                    customer = new Customer
                    {
                        FullName = payload.Name,
                        Email = payload.Email,
                        Image = payload.Picture,
                        Phone = "",
                        Address = "",
                        IsActive = true,
                        CreateOn = DateTime.UtcNow
                    };

                    _dbContext.Customers.Add(customer);
                    await _dbContext.SaveChangesAsync();
                }

                return Ok(new ResultCustomModel<object>
                {
                    Code = 200,
                    Success = true,
                    Message = "Đăng nhập Google thành công.",
                    Data = new
                    {
                        customerId = customer.CustomerId,
                        fullName = customer.FullName,
                        email = customer.Email,
                        image = customer.Image
                    }
                });
            }
            catch (InvalidJwtException)
            {
                return Unauthorized(new ResultCustomModel<object>
                {
                    Code = 401,
                    Success = false,
                    Message = "Google Token không hợp lệ."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultCustomModel<object>
                {
                    Code = 500,
                    Success = false,
                    Message = $"Lỗi server: {ex.Message}"
                });
            }
        }
    }

    public class GoogleSignInRequest
    {
        public string IdToken { get; set; }
    }
}
