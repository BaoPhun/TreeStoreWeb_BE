using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TreeStore.Models.CustomModels;
using TreeStore.Models.Entities;
using TreeStore.Models.LoginModels;
using TreeStore.Models.UserModels;
using TreeStore.Services;
using TreeStore.Services.Interfaces;
using TreeStore.Utilities;

namespace TreeStore.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    
    public class UserController : Controller
    {
        private readonly IUserServices _userServices;
        public UserController(IUserServices userServices)
        {
            _userServices = userServices;
        }


        [HttpPost]
        public async Task<ResultCustomModel<bool>> Create([FromBody] CreateUserRequest request)
        {
            return await _userServices.CreateAsync(request);
        }

        [HttpGet]
        public async Task<ResultCustomModel<List<UpdateUserResponse>>> ListUser()
        {
            return await _userServices.ListUsersAsync();
        }


        [HttpPut]
        public async Task<ResultCustomModel<UpdateUserResponse>> UpdateUser([FromBody] UpdateUserRequest updateUserRequest)
        {
            var result = await _userServices.UpdateUserAsync(updateUserRequest);
            return result;
        }
        [HttpPost]
        public async Task<IActionResult> DeactivateAccount([FromBody] DeactivateAccountRequest request)
        {
            try
            {
                await _userServices.DeactivateAccountAsync(request.UserId);

                return Ok(new ResultCustomModel<string>
                {
                    Code = 200,
                    Data = "Tài khoản đã bị vô hiệu hóa thành công.",
                    Success = true,
                    Message = "Vô hiệu hóa tài khoản thành công."
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
        public async Task<ResultCustomModel<string>> ActivateAccount([FromBody] int userId)
        {
            try
            {
                await _userServices.ActivateAccountAsync(userId);

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
        public async Task<ResultCustomModel<bool>> ChangeActive([FromQuery] int userId)
        {
            return await _userServices.ChangeActiveAsync(userId);
        }

        [HttpGet]
        public async Task<ResultCustomModel<UserResponse>> GetUserById([FromQuery] int userId)
        {
            return await _userServices.GetUserByIdAsync(userId);
        }
        [HttpGet]
        public async Task<ResultCustomModel<List<Role>>> GetRoles()
        {
            return await _userServices.GetRolesAsync();
        }
        [HttpGet]
        public async Task<ResultCustomModel<List<UserResponse>>> SearchByName(string name)
        {
            var result = await _userServices.SearchUserByNameAsync(name);
            return new ResultCustomModel<List<UserResponse>>
            {
                Data = result,
                Success = result.Any(),
                Message = result.Any() ? "Tìm thấy người dùng" : "Không tìm thấy người dùng"
            };
        }

    }
}