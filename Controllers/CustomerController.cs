using Microsoft.AspNetCore.Mvc;
using TreeStore.Services;
using TreeStore.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using TreeStore.Models.CustomModels;
using TreeStore.Models.Entities;
using TreeStore.Models.CustomerModels;


namespace TreeStore.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]

    public class CustomerController : Controller
    {
        private readonly ICustomerServices _customerServices;

        public CustomerController(ICustomerServices customerServices)
        {
            _customerServices = customerServices;   
        }
       
        [HttpGet]
        public async Task<ResultCustomModel<List<CustomerResponse>>> ListCustomer()
        {
            return await _customerServices.ListCustomersAsync(); 
        }
        [HttpGet]
        public async Task<ResultCustomModel<int>> GetCurrentCustomerIdAsync(string email)
        {
            return await _customerServices.GetCurrentCustomerIdAsync(email);    
        }
        [HttpGet]
        public async Task<ResultCustomModel<CustomerResponse>> GetCustomerById([FromQuery] int customerId)
        {
            return await _customerServices.GetCustomerByIdAsync(customerId);
        }


        [HttpPut]
        public async Task<ResultCustomModel<CustomerResponse>> UpdateCustomer([FromBody] UpdateCustomerRequest updateCustomerRequest)
        {
            var result = await _customerServices.UpdateCustomer(updateCustomerRequest);
            return result;
        }


        [HttpPost]
        public async Task<IActionResult> DeactivateAccount([FromBody] DeactivateCustomerRequest request)
        {
            try
            {
                await _customerServices.DeactivateAccountAsync(request.CustomerId);

                return Ok(new ResultCustomModel<string>
                {
                    Code = 200,
                    Data = "Khách hàng đã bị vô hiệu hóa thành công.",
                    Success = true,
                    Message = "Vô hiệu khách hàng thành công."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResultCustomModel<string>
                {
                    Code = 400,
                    Data = null,
                    Success = false,
                    Message = ex.Message
                });
            }
        }



        [HttpPost]
        public async Task<ResultCustomModel<string>> ActivateAccount([FromBody] int customerId)
        {
            try
            {
                await _customerServices.ActivateAccountAsync(customerId);

                return new ResultCustomModel<string>
                {
                    Code = 200,
                    Data = "Tài khoản đã được kích hoạt thành công.",
                    Success = true,
                    Message = "Kích hoạt tài khoản thành công"
                };
            }
            catch (Exception ex)
            {
                return new ResultCustomModel<string>
                {
                    Code = 400,
                    Data = null,
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        [HttpPost]
        public async Task<ResultCustomModel<bool>> ChangeActive([FromQuery] int customerId)
        {
            return await _customerServices.ChangeActiveAsync(customerId);
        }

        [HttpGet]
        public async Task<IActionResult> SearchByName(string name)
        {
            var result = await _customerServices.SearchCustomerByNameAsync(name);
            if (result.Success)
            {
                return Ok(result);
            }
            return NotFound(new { message = "Không tìm thấy khách hàng." });
        }
        [HttpPost]
        public async Task<ResultCustomModel<CustomerResponse>> Register([FromBody] CustomerRequest customerRequest)
        {
            return await _customerServices.RegisterCustomerAsync(customerRequest);
        }

        [HttpPost]
        public async Task<ActionResult<ResultCustomModel<CustomerResponse>>> Login([FromBody] LoginCustomer loginCustomer)
        {
            // Kiểm tra nếu loginCustomer là null hoặc các trường Email và Password không hợp lệ
            if (loginCustomer == null)
            {
                return BadRequest(new ResultCustomModel<CustomerResponse>
                {
                    Code = 400,
                    Success = false,
                    Message = "Thông tin đăng nhập không hợp lệ."
                });
            }

            if (string.IsNullOrWhiteSpace(loginCustomer.Email) || string.IsNullOrWhiteSpace(loginCustomer.Password))
            {
                return BadRequest(new ResultCustomModel<CustomerResponse>
                {
                    Code = 400,
                    Success = false,
                    Message = "Email và mật khẩu không được để trống."
                });
            }

            // Gọi phương thức LoginAsync để xử lý đăng nhập
            var result = await _customerServices.LoginAsync(loginCustomer);

            if (result.Success)
            {
                return Ok(result); // Trả về mã 200 nếu đăng nhập thành công
            }
            else
            {
                return Unauthorized(result); // Trả về mã 401 nếu đăng nhập thất bại
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            var result = await _customerServices.LogoutAsync();

            if (result.Success)
            {
                return Ok(new
                {
                    code = result.Code,
                    message = result.Message
                });
            }

            return Unauthorized(new
            {
                code = 401,
                message = "Đăng xuất không thành công"
            });
        }
        [HttpPost("ForgotPassword")]
        public async Task<ActionResult<ResultCustomModel<string>>> ForgotPasswordAsync( string email)
        {
            // Gọi phương thức ForgotPasswordAsync từ service để gửi OTP
            var result = await _customerServices.ForgotPasswordAsync(email);

            if (result.Success)
            {
                return Ok(result); // Trả về kết quả thành công
            }

            // Nếu email không tồn tại, trả về đối tượng ResultCustomModel với mã lỗi 404
            return NotFound(new ResultCustomModel<string>
            {
                Code = 404,
                Success = false,
                Message = result.Message
            });
        }

        [HttpPost("ResetPassword")]
        public async Task<ActionResult<ResultCustomModel<string>>> ResetPasswordAsync( string otp, string newPassword)
        {
            // Gọi phương thức ResetPasswordAsync từ service để đặt lại mật khẩu
            var result = await _customerServices.ResetPasswordAsync( otp, newPassword);

            if (result.Success)
            {
                return Ok(result); // Trả về kết quả thành công
            }

            // Nếu có lỗi, trả về đối tượng ResultCustomModel với mã lỗi 400
            return BadRequest(new ResultCustomModel<string>
            {
                Code = 400,
                Success = false,
                Message = result.Message
            });
        }

        [HttpGet("total-customer")]
        public async Task<ResultCustomModel<int>> GetTotalCustomers()
        {
            // Gọi service để tính tổng số đơn hàng
            var result = await _customerServices.GetTotalCustomersAsync();

            // Trả về kết quả (response) cho client
            return result;
        }


        [HttpPost("UploadAvatar")]
        public async Task<ActionResult> UploadAvatar(int customerId, IFormFile avatar)
        {
            try
            {
                var imageUrl = await _customerServices.UploadAvatar(customerId, avatar);
                return Ok(new { imageUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
