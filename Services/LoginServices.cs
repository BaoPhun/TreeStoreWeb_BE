using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TreeStore.Models.CustomModels;
using TreeStore.Models.Entities;
using TreeStore.Models.LoginModels;
using TreeStore.Services.Interfaces;
using TreeStore.Utilities;

namespace TreeStore.Services
{
    public class LoginServices : BaseServices, ILoginServices
    {
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        public LoginServices(TreeStoreDBContext db, ITreeStoreDBContextProcedures sp, IConfiguration config, IMapper mapper) : base(db, sp)
        {
            _config = config;
            _mapper = mapper;
        }

        public async Task<ResultCustomModel<LoginResponse>> SignInAsync(LoginRequest login)
        {
                Vlogin user = await _db.Vlogins.FirstOrDefaultAsync(x => x.Email == login.Email);
                if (user != null)
                {
               
                    if (!user.Password.IsEqualPassword(login.Password))
                    {
                        return new ResultCustomModel<LoginResponse>
                        {
                            Code = 400,
                            Data = null,
                            Success = false,
                            Message = "Sai tên tài khoản hoặc mật khẩu"
                        };
                    }
                    LoginResponse response = _mapper.Map<LoginResponse>(user);
                    (string Token, string[] ListRole) jwt = GenerateJWT(user);
                    response.Token = jwt.Token;
                    response.Role = jwt.ListRole;
                    response.RoleName = jwt.ListRole.FirstOrDefault();
                    return new ResultCustomModel<LoginResponse>
                    {
                        Code = 200,
                        Data = response,
                        Success = true,
                        Message = "Ok"
                    }; ;
                }
            
            return new ResultCustomModel<LoginResponse>
            {
                Code = 400,
                Data = null,
                Success = false,
                Message = "Sai tên tài khoản hoặc mật khẩu"
            };
        }


        public async Task<ResultCustomModel<RegisterResponse>> RegisterAsync(RegisterRequest registerRequest)
        {
            if (!registerRequest.Email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                return new ResultCustomModel<RegisterResponse>
                {
                    Code = 400,
                    Data = null,
                    Success = false,
                    Message = "Email phải có đuôi @gmail.com"
                };
            }
            if (registerRequest.Password.Length <= 5)
            {
                return new ResultCustomModel<RegisterResponse>
                {
                    Code = 400,
                    Data = null,
                    Success = false,
                    Message = "Mật khẩu phải có độ dài lớn hơn 5 ký tự"
                };
            }
            var existingUser = await _db.Users.FirstOrDefaultAsync(x => x.Username == registerRequest.Username);
            if (existingUser != null)
            {
                return new ResultCustomModel<RegisterResponse>
                {
                    Code = 400,
                    Data = null,
                    Success = false,
                    Message = "Tên tài khoản đã tồn tại"
                };
            }

            // Hash mật khẩu trước khi lưu
            var hashedPassword = registerRequest.Password.ToScryptEncode();

            // Tạo người dùng mới
            var newUser = new User
            {
                Username = registerRequest.Username,
                Password = hashedPassword,
                Phone = registerRequest.Phone,
                Email = registerRequest.Email,
                Address = registerRequest.Address,
                Birthday = registerRequest.Birthday,
                Fullname = registerRequest.Fullname,
                Position = registerRequest.Position,
                CreateOn = DateTime.Now,
                IsActive = true
            };

            _db.Users.Add(newUser);
            await _db.SaveChangesAsync();

            var response = new RegisterResponse
            {
                UserId = newUser.UserId,
                Username = newUser.Username,
                Fullname = newUser.Fullname
            };

            return new ResultCustomModel<RegisterResponse>
            {
                Code = 200,
                Data = response,
                Success = true,
                Message = "Đăng ký thành công"
            };
        }

        private (string Token, string[] ListRole) GenerateJWT(Vlogin user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim("UserName", user.Username),
                new Claim("FullName", user.Fullname)
            };
            var listRoles = user?.ListRoleName.Split(',');
            foreach (string item in listRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, item));
            }
            DateTime expire = DateTime.Now.AddHours(12);
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims,
                expires: expire,
                signingCredentials: credentials);
            return (new JwtSecurityTokenHandler().WriteToken(token), listRoles);
        }
    }
}
