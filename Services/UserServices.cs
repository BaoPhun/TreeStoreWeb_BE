using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TreeStore.Models.CustomModels;
using TreeStore.Models.Entities;
using TreeStore.Models.LoginModels;
using TreeStore.Models.UserModels;
using TreeStore.Services.Interfaces;
using TreeStore.Utilities;

namespace TreeStore.Services
{
    public class UserServices : BaseServices, IUserServices
    {

        public UserServices(TreeStoreDBContext db, ITreeStoreDBContextProcedures sp) : base(db, sp)
        {
        }
        public async Task ActivateAccountAsync(int userId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if (user == null)
            {
                throw new Exception("Người dùng không tồn tại");
            }

            user.IsActive = true;
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
        }

        public async Task DeactivateAccountAsync(int userId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if (user == null)
            {
                throw new Exception("Người dùng không tồn tại");
            }

            user.IsActive = false;
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
        }
        public async Task<ResultCustomModel<List<UpdateUserResponse>>> ListUsersAsync()
        {
            // Lấy danh sách người dùng từ database
            List<User> users = await _db.Users.ToListAsync();

            // Chuyển đổi danh sách User thành danh sách UpdateUserResponse
            List<UpdateUserResponse> userResponses = users.Select(user => new UpdateUserResponse
            {
                UserId = user.UserId,
                Username = user.Username,
                Fullname = user.Fullname,
                Avatar = user.Avatar,
                Phone = user.Phone,
                IsActive = user.IsActive,
                Email = user.Email,
                Address = user.Address,
                Position = user.Position,
                Birthday = user.Birthday
            }).ToList();

            return new ResultCustomModel<List<UpdateUserResponse>>
            {
                Code = 200,
                Data = userResponses,
                Success = true,
                Message = "Thành công!"
            };
        }

        public async Task<ResultCustomModel<UpdateUserResponse>> UpdateUserAsync(UpdateUserRequest request)
        {
            // Tìm người dùng theo ID
            User user = await _db.Users.FindAsync(request.UserId);

            if (user == null)
            {
                return new ResultCustomModel<UpdateUserResponse>
                {
                    Code = 404,
                    Success = false,
                    Message = "Người dùng không tồn tại"
                };
            }

            // Cập nhật thông tin người dùng
            //user.Username = request.Username;
            user.Fullname = request.Fullname;
            //if (!string.IsNullOrEmpty(request.Password))
            //{
            //    user.Password = request.Password.ToScryptEncode(); // Hash mật khẩu trước khi lưu
            //}

            //user.Avatar = request.Avatar;
            user.Phone = request.Phone;
            user.Email = request.Email;
            user.Address = request.Address;
            user.Birthday = request.Birthday;
            user.IsActive = request.IsActive;
            string positionFromRole = string.Join(", ",
               _db.Roles
                  .Where(x => request.ListRoles.Contains(x.RoleId))
                  .Select(z => z.Name)
                  .ToList()
            );
            user.Position = positionFromRole;
            _db.Users.Update(user);

            await _db.SaveChangesAsync();


            List<UserRole> userRolesOld = _db.UserRoles.Where(x => x.UserId == request.UserId).ToList();
            _db.UserRoles.RemoveRange(userRolesOld);
            await _db.SaveChangesAsync();

            List<UserRole> userRolesNew = new List<UserRole>();
            request.ListRoles.ForEach(x => userRolesNew.Add(new UserRole()
            {
                RoleId = x,
                UserId = request.UserId,
            }));

            await _db.UserRoles.AddRangeAsync(userRolesNew);
            await _db.SaveChangesAsync();

            // Tạo đối tượng UpdateUserResponse sau khi cập nhật người dùng
            var updateUserResponse = new UpdateUserResponse
            {
                UserId = user.UserId,
                Username = user.Username,
                Fullname = user.Fullname,
                IsActive = request.IsActive,
                Avatar = user.Avatar,
                Phone = user.Phone,
                Email = user.Email,
                Address = user.Address,
                Birthday = user.Birthday
            };

            return new ResultCustomModel<UpdateUserResponse>
            {
                Code = 200,
                Data = updateUserResponse,
                Success = true,
                Message = "Cập nhật thành công"
            };
        }
        //public async Task<ResultCustomModel<string>> DeactivateAccountAsync(int userId)
        //{
        //    var user = await _db.Users.FirstOrDefaultAsync(x => x.UserId == userId);
        //    if (user == null)
        //    {
        //        return new ResultCustomModel<string>
        //        {
        //            Code = 404,
        //            Data = null,
        //            Success = false,
        //            Message = "Người dùng không tồn tại"
        //        };
        //    }

        //    user.IsActive = false;
        //    _db.Users.Update(user);
        //    await _db.SaveChangesAsync();

        //    return new ResultCustomModel<string>
        //    {
        //        Code = 200,
        //        Data = "Tài khoản đã bị vô hiệu hóa",
        //        Success = true,
        //        Message = "Vô hiệu hóa tài khoản thành công"
        //    };
        //}

        //public async Task<ResultCustomModel<string>> ActivateAccountAsync(int userId)
        //{
        //    var user = await _db.Users.FirstOrDefaultAsync(x => x.UserId == userId);
        //    if (user == null)
        //    {
        //        return new ResultCustomModel<string>
        //        {
        //            Code = 404,
        //            Data = null,
        //            Success = false,
        //            Message = "Người dùng không tồn tại"
        //        };
        //    }

        //    user.IsActive = true;
        //    _db.Users.Update(user);
        //    await _db.SaveChangesAsync();

        //    return new ResultCustomModel<string>
        //    {
        //        Code = 200,
        //        Data = "Tài khoản đã được kích hoạt",
        //        Success = true,
        //        Message = "Kích hoạt tài khoản thành công"
        //    };
        //}

        public async Task<ResultCustomModel<bool>> CreateAsync(CreateUserRequest request)
        {
            // Tìm người dùng theo ID
            bool isExistUser = await _db.Users.AnyAsync(x => x.Email.Equals(request.Email) || x.Username.Equals(request.Username));

            if (isExistUser)
            {
                return new ResultCustomModel<bool>
                {
                    Code = 404,
                    Success = false,
                    Message = "Tài khoản hoặc email này đã tồn tại!"
                };
            }
            List<int> roles = await _db.Roles.Select(x => x.RoleId).ToListAsync();
            List<int> roleInvalidFromInput = request.ListRoles.Where(x => !roles.Contains(x)).ToList();
            if (roleInvalidFromInput.Count > 0)
            {
                return new ResultCustomModel<bool>
                {
                    Code = 404,
                    Success = false,
                    Message = "Giá trị các quyền nhập vào không hợp lệ, không tồn tại trong hệ thống."
                };
            }
            // Hash mật khẩu trước khi lưu
            //var hashedPassword = request.Password.ToScryptEncode();
            var hashedPassword = "123456".ToScryptEncode();
            string positionFromRole = string.Join(", ",
                _db.Roles
                   .Where(x => request.ListRoles.Contains(x.RoleId))
                   .Select(z => z.Name)
                   .ToList()
            );
            // Tạo người dùng mới
            var newUser = new User
            {
                Username = request.Username,
                Password = hashedPassword,
                Phone = request.Phone,
                Email = request.Email,
                Address = request.Address,
                Birthday = request.Birthday,
                Fullname = request.Fullname,
                Position = positionFromRole,
                CreateOn = DateTime.Now,
                IsActive = true,
                Avatar = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTcZsL6PVn0SNiabAKz7js0QknS2ilJam19QQ&s",
                UserRoles = request.ListRoles.Select(roleId => new UserRole
                {
                    RoleId = roleId,
                    // newUser.UserId sẽ tự động cập nhật khi SaveChangesAsync được gọi
                }).ToList()
            };
            _db.Users.Add(newUser);
            await _db.SaveChangesAsync();

            List<UserRole> UserRoleNew = new List<UserRole>();

            foreach (int roleId in request.ListRoles)
            {
                UserRoleNew.Add(new UserRole()
                {
                    RoleId = roleId,
                    UserId = newUser.UserId
                });
            }

            await _db.UserRoles.AddRangeAsync(UserRoleNew);
            int result = await _db.SaveChangesAsync();
            bool isSuccess = result > 0;

            return new ResultCustomModel<bool>
            {
                Code = isSuccess ? 201 : 400,
                Success = isSuccess,
                Message = $"Tạo tài khoản {(isSuccess ? "thành công" : "thất bại")} - Mật khẩu mặc định hiện tại là 123456"
            };
        }

        public async Task<ResultCustomModel<UserResponse>> GetUserByIdAsync(int userId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            bool hasUser = user != null;

            UserResponse userResponse = default;

            if (hasUser)
            {
                userResponse = new UserResponse()
                {
                    UserId = userId,
                    Username = user.Username,
                    Fullname = user.Fullname,
                    Avatar = user.Avatar,
                    Phone = user.Phone,
                    Email = user.Email,
                    Address = user.Address,
                    Birthday = user.Birthday,
                    Position = user.Position,
                    IsActive = user.IsActive,
                    CreateOn = user.CreateOn,
                };
                List<int> lstRoles = _db.UserRoles.Where(x => x.UserId == userId).Select(z => z.RoleId).ToList();
                userResponse.lstRolesId = lstRoles;
            }

            return new ResultCustomModel<UserResponse>
            {
                Code = hasUser ? 200 : 404,
                Data = userResponse,
                Success = hasUser,
                Message = hasUser ? "Lấy thông tin người dùng thành công" : "Người dùng không tồn tại"
            };
        }

        public async Task<ResultCustomModel<List<Role>>> GetRolesAsync()
        {
            return new ResultCustomModel<List<Role>>
            {
                Code = 200,
                Data = await _db.Roles.ToListAsync(),
                Success = true,
                Message = "Lấy DS Role thành công!"
            };
        }

        public async Task<ResultCustomModel<bool>> ChangeActiveAsync(int userId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            bool hasUser = user != null;
            bool isSuccess = false;
            if (hasUser)
            {
                user.IsActive = !user.IsActive;
                _db.Entry(user).State = EntityState.Modified;
                _db.Users.Update(user);
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

        //tìm kiếm
        public async Task<List<UserResponse>> SearchUserByNameAsync(string name)
        {
            var users = await _db.Users
                .Where(u => u.Fullname.Contains(name))
                .Select(u => new UserResponse
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Phone = u.Phone,
                    Email = u.Email,
                    Address = u.Address,
                    Birthday = u.Birthday,
                    IsActive = u.IsActive,
                    CreateOn = u.CreateOn,
                    Fullname = u.Fullname,
                    Avatar = u.Avatar,
                    Position = u.Position,
                    lstRolesId = u.UserRoles.Select(ur => ur.RoleId).ToList()
                })
                .ToListAsync();

            return users;
        }

    }
}
