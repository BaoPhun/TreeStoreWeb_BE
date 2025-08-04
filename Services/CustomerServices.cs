using Microsoft.EntityFrameworkCore;
using TreeStore.Models.CustomerModels;
using TreeStore.Models.CustomModels;
using TreeStore.Models.Entities;
using TreeStore.Services.Interfaces;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using TreeStore.Utilities;
using System.Text.RegularExpressions;

namespace TreeStore.Services
{
    public class CustomerServices : BaseServices, ICustomerServices
    {
        public CustomerServices(TreeStoreDBContext db, ITreeStoreDBContextProcedures sp) : base(db, sp)
        {
        }
        private static Dictionary<string, (string otp, DateTime expiration)> otpStore = new Dictionary<string, (string otp, DateTime expiration)>();
        public async Task<ResultCustomModel<List<CustomerResponse>>> ListCustomersAsync()
        {
            // Lấy danh sách khách hàng từ cơ sở dữ liệu
            List<Customer> customers = await _db.Customers.ToListAsync();

            // Chuyển đổi danh sách Customer thành danh sách CustomerResponse bằng LINQ
            var customerResponses = customers.Select(customer => new CustomerResponse
            {
                CustomerId = customer.CustomerId,
                Fullname = customer.FullName,
                Phone = customer.Phone,
                Email = customer.Email,
                Address = customer.Address,
                IsActive = customer.IsActive // Đây sẽ giữ nguyên giá trị của IsActive từ database, dù là null hay true/false
            }).ToList();

            // Trả về kết quả
            return new ResultCustomModel<List<CustomerResponse>>
            {
                Code = 200,
                Data = customerResponses,
                Success = true,
                Message = "Thành công!"
            };
        }

        public async Task<ResultCustomModel<CustomerResponse>> GetCustomerByIdAsync(int customerId)
        {
            var customer = await _db.Customers.FirstOrDefaultAsync(x => x.CustomerId == customerId);

            /* var  customer = await _db.Customers.FirstOrDefaultAsync(x => x.CustomerId == customerId)*/
            
            bool hasCustomer = customer != null;


                
            CustomerResponse customerResponse = new CustomerResponse();
            if (hasCustomer)
            {
                customerResponse = mapToCustomer(customer);
            }

            return new ResultCustomModel<CustomerResponse>
            {
                Code = hasCustomer ? 200 : 404,
                Data = customerResponse,
                Success = hasCustomer,
                Message = hasCustomer ? "Lấy thông tin khách hàng thành công" : "Khách hàng không tồn tại"
            };
        }

        public async Task<ResultCustomModel<int>> GetCurrentCustomerIdAsync(string email)
        {
            // Kiểm tra xem email có hợp lệ không
            if (string.IsNullOrWhiteSpace(email))
            {
                return new ResultCustomModel<int>
                {
                    Code = 400,
                    Success = false,
                    Message = "Email không hợp lệ"
                };
            }

            // Tìm khách hàng theo email
            var customer = await _db.Customers.FirstOrDefaultAsync(x => x.Email == email);

            if (customer == null)
            {
                return new ResultCustomModel<int>
                {
                    Code = 404,
                    Success = false,
                    Message = "Khách hàng không tồn tại"
                };
            }

            // Trả về ID của khách hàng
            return new ResultCustomModel<int>
            {
                Code = 200,
                Data = customer.CustomerId,
                Success = true,
                Message = "Lấy ID khách hàng thành công"
            };
        }

       

        public async Task<ResultCustomModel<CustomerResponse>> UpdateCustomer(UpdateCustomerRequest request)
        {
            // Tìm khách hàng dựa trên CustomerId
            Customer customer = await _db.Customers.FindAsync(request.CustomerId);

            if (customer == null)
            {
                return new ResultCustomModel<CustomerResponse>
                {
                    Code = 404,
                    Success = false,
                    Message = "Khách hàng không tồn tại"
                };
            }

            // Validate các trường dữ liệu
            if (string.IsNullOrEmpty(request.Fullname) || request.Fullname.Length < 3)
            {
                return new ResultCustomModel<CustomerResponse>
                {
                    Code = 400,
                    Success = false,
                    Message = "Tên khách hàng không hợp lệ. Tên phải có ít nhất 3 ký tự."
                };
            }

            // Kiểm tra số điện thoại
            if (string.IsNullOrEmpty(request.Phone) || !Regex.IsMatch(request.Phone, @"^\d{10,15}$"))
            {
                return new ResultCustomModel<CustomerResponse>
                {
                    Code = 400,
                    Success = false,
                    Message = "Số điện thoại không hợp lệ. Số điện thoại phải có từ 10 đến 15 chữ số."
                };
            }

            // Kiểm tra email
            if (string.IsNullOrEmpty(request.Email) || !Regex.IsMatch(request.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return new ResultCustomModel<CustomerResponse>
                {
                    Code = 400,
                    Success = false,
                    Message = "Email không hợp lệ."
                };
            }

            // Kiểm tra trùng email với khách hàng khác
            var existingCustomerWithEmail = await _db.Customers
                .Where(c => c.Email == request.Email && c.CustomerId != request.CustomerId)
                .FirstOrDefaultAsync();
            if (existingCustomerWithEmail != null)
            {
                return new ResultCustomModel<CustomerResponse>
                {
                    Code = 400,
                    Success = false,
                    Message = "Email đã được sử dụng bởi một khách hàng khác."
                };
            }

            // Kiểm tra trùng số điện thoại với khách hàng khác
            var existingCustomerWithPhone = await _db.Customers
                .Where(c => c.Phone == request.Phone && c.CustomerId != request.CustomerId)
                .FirstOrDefaultAsync();
            if (existingCustomerWithPhone != null)
            {
                return new ResultCustomModel<CustomerResponse>
                {
                    Code = 400,
                    Success = false,
                    Message = "Số điện thoại đã được sử dụng bởi một khách hàng khác."
                };
            }

            // Kiểm tra địa chỉ có hợp lệ không
            if (string.IsNullOrEmpty(request.Address) || request.Address.Length < 5)
            {
                return new ResultCustomModel<CustomerResponse>
                {
                    Code = 400,
                    Success = false,
                    Message = "Địa chỉ không hợp lệ. Địa chỉ phải có ít nhất 5 ký tự."
                };
            }

            // Kiểm tra xem có thay đổi dữ liệu nào không
            bool isChanged = false;

            if (customer.FullName != request.Fullname)
            {
                customer.FullName = request.Fullname;
                isChanged = true;
            }

            if (customer.Phone != request.Phone)
            {
                customer.Phone = request.Phone;
                isChanged = true;
            }

            if (customer.Email != request.Email)
            {
                customer.Email = request.Email;
                isChanged = true;
            }

            if (customer.Address != request.Address)
            {
                customer.Address = request.Address;
                isChanged = true;
            }

            if (!string.IsNullOrEmpty(request.Password))
            {
                if (request.Password.Length < 6)
                {
                    return new ResultCustomModel<CustomerResponse>
                    {
                        Code = 400,
                        Success = false,
                        Message = "Mật khẩu phải có ít nhất 6 ký tự."
                    };
                }

                try
                {
                    var encoder = new Scrypt.ScryptEncoder();

                    // Kiểm tra mật khẩu mới có giống mật khẩu cũ không
                    if (!encoder.Compare(request.Password, customer.Password))
                    {
                        customer.Password = encoder.Encode(request.Password); // chỉ hash nếu khác
                        isChanged = true;
                    }
                }
                catch (Exception ex)
                {
                    return new ResultCustomModel<CustomerResponse>
                    {
                        Code = 500,
                        Success = false,
                        Message = "Lỗi khi cập nhật mật khẩu: " + ex.Message
                    };
                }
            }



            // Nếu không có gì thay đổi, trả về thông báo
            if (!isChanged)
            {
                return new ResultCustomModel<CustomerResponse>
                {
                    Code = 304, // 304: Not Modified
                    Success = true,
                    Message = "Không có gì thay đổi"
                };
            }
            // Cập nhật dữ liệu khách hàng
            _db.Customers.Update(customer);

            await _db.SaveChangesAsync();

            // Tạo đối tượng phản hồi
            var updateCustomerResponse = new CustomerResponse
            {
                CustomerId = customer.CustomerId,
                Fullname = customer.FullName,
                Phone = customer.Phone,
                Email = customer.Email,
                Address = customer.Address,
            };

            return new ResultCustomModel<CustomerResponse>
            {
                Code = 200,
                Data = updateCustomerResponse,
                Success = true,
                Message = "Cập nhật thành công"
            };
        }

        private CustomerResponse mapToCustomer(Customer customer)
        {

            return new CustomerResponse
            {

                CustomerId = customer.CustomerId,
                Fullname = customer.FullName,
                Phone = customer.Phone,
                Address = customer.Address,
                Image = customer.Image,
                Email = customer.Email,


            };
        }
        public async Task ActivateAccountAsync(int custonmerId)
        {
            var customer = await _db.Customers.FirstOrDefaultAsync(x => x.CustomerId == custonmerId);
            if (customer == null)
            {
                throw new Exception("Người dùng không tồn tại");
            }

            customer.IsActive = true;
            _db.Customers.Update(customer);
            await _db.SaveChangesAsync();
        }

        public async Task DeactivateAccountAsync(int custonmerId)
        {
            var customer = await _db.Customers.FirstOrDefaultAsync(x => x.CustomerId == custonmerId);
            if (customer == null)
            {
                throw new Exception("Người dùng không tồn tại");
            }

            customer.IsActive = false;
            _db.Customers.Update(customer);
            await _db.SaveChangesAsync();
        }

        public async Task<ResultCustomModel<bool>> ChangeActiveAsync(int customerId)
        {
            var customer = await _db.Customers.FirstOrDefaultAsync(x => x.CustomerId == customerId);

            if (customer == null)
            {
                return new ResultCustomModel<bool>
                {
                    Code = 404,
                    Data = false,
                    Success = false,
                    Message = "Khách hàng không tồn tại"
                };
            }

            customer.IsActive = !customer.IsActive;
            _db.Entry(customer).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return new ResultCustomModel<bool>
            {
                Code = 200,
                Data = true,
                Success = true,
                Message = "Cập nhật trạng thái thành công"
            };
        }



        public async Task<ResultCustomModel<List<CustomerResponse>>> SearchCustomerByNameAsync(string name)
        {
            var customers = await _db.Customers
                .Where(c => c.FullName.Contains(name))
                .Select(c => new CustomerResponse
                {
                    CustomerId = c.CustomerId,
                    Fullname = c.FullName,
                    //Avatar = c.Image,
                    IsActive = c.IsActive,
                    Phone = c.Phone,
                    Email = c.Email,
                    Address = c.Address
                })
                .ToListAsync();

            return new ResultCustomModel<List<CustomerResponse>>
            {
                Success = customers.Any(),
                Data = customers,
                Message = customers.Any() ? "Tìm thấy khách hàng." : "Không tìm thấy khách hàng."
            };
        }


        public async Task<ResultCustomModel<CustomerResponse>> RegisterCustomerAsync(CustomerRequest customerRequest)
        {
            // Kiểm tra email có đúng định dạng không
            if (!customerRequest.Email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                return new ResultCustomModel<CustomerResponse>
                {
                    Code = 400,
                    Data = null,
                    Success = false,
                    Message = "Email phải có đuôi @gmail.com"
                };
            }

            // Kiểm tra độ dài mật khẩu
            if (customerRequest.Password.Length <= 5)
            {
                return new ResultCustomModel<CustomerResponse>
                {
                    Code = 400,
                    Data = null,
                    Success = false,
                    Message = "Mật khẩu phải có độ dài lớn hơn 5 ký tự"
                };
            }

            // Kiểm tra tên người dùng đã tồn tại chưa
            var existingCustomer = await _db.Customers.FirstOrDefaultAsync(x => x.Username == customerRequest.Username);
            if (existingCustomer != null)
            {
                return new ResultCustomModel<CustomerResponse>
                {
                    Code = 400,
                    Data = null,
                    Success = false,
                    Message = "Tên tài khoản đã tồn tại"
                };
            }

            // Băm mật khẩu trước khi lưu
            var hashedPassword = customerRequest.Password.ToScryptEncode(); // Giả sử bạn có phương thức băm mật khẩu

            // Tạo khách hàng mới
            var newCustomer = new Customer
            {
                Username = customerRequest.Username,
                Password = hashedPassword,
                Phone = customerRequest.Phone,
                Email = customerRequest.Email,
                Address = customerRequest.Address,
                FullName = customerRequest.FullName,
                CreateOn = DateTime.Now,
                IsActive = true
            };

            _db.Customers.Add(newCustomer);
            await _db.SaveChangesAsync();

            var response = new CustomerResponse
            {
                CustomerId = newCustomer.CustomerId,
                Fullname = newCustomer.FullName,
                Phone = newCustomer.Phone,
                Email = newCustomer.Email,
                Address = newCustomer.Address
            };

            return new ResultCustomModel<CustomerResponse>
            {
                Code = 200,
                Data = response,
                Success = true,
                Message = "Đăng ký thành công"
            };
        }

        public async Task<ResultCustomModel<CustomerResponse>> LoginAsync(LoginCustomer loginCustomer)
        {
            // Lấy thông tin khách hàng theo email
            var customer = await _db.Customers.FirstOrDefaultAsync(x => x.Email == loginCustomer.Email);

            // Kiểm tra xem khách hàng có tồn tại và mật khẩu có hợp lệ không
            bool hasCustomer = customer != null && customer.Password.IsEqualPassword(loginCustomer.Password) && customer.IsActive == true;

            CustomerResponse customerResponse = new CustomerResponse();

            if (hasCustomer)
            {
                customerResponse = mapToCustomer(customer);
            }

            return new ResultCustomModel<CustomerResponse>
            {
                Code = hasCustomer ? 200 : 401, // 200 nếu thành công, 401 nếu không hợp lệ
                Data = customerResponse,
                Success = hasCustomer,
                Message = hasCustomer ? "Đăng nhập thành công" : "Email hoặc mật khẩu không đúng"
            };
        }
        public async Task<ResultCustomModel<string>> LogoutAsync()
        {
            // Thực hiện các thao tác liên quan đến đăng xuất như xoá thông tin phiên hoặc token
            // Nếu cần, bạn có thể cập nhật trạng thái đăng nhập của khách hàng trong cơ sở dữ liệu

            // Ở đây có thể thêm các logic xóa session, cookies, hoặc token liên quan đến khách hàng

            return new ResultCustomModel<string>
            {
                Code = 200, // 200 khi đăng xuất thành công
                Data = null,
                Success = true,
                Message = "Đăng xuất thành công"
            };
        }


        private string GenerateOTP()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString(); // Tạo mã OTP 6 chữ số
        }

        // Gửi mã OTP đến email
        private async Task SendOtpEmailAsync(string email, string otp)
        {
            await EmailService.SendEmailAsync(email, "Mã OTP của bạn", $"Mã OTP để đặt lại mật khẩu là: {otp}");
        }

        public async Task<ResultCustomModel<string>> ForgotPasswordAsync(string email)
        {
            // Kiểm tra xem email có hợp lệ không
            var customer = await _db.Customers.FirstOrDefaultAsync(x => x.Email == email);
            if (customer == null)
            {
                return new ResultCustomModel<string>
                {
                    Code = 404,
                    Success = false,
                    Message = "Email không tồn tại trong hệ thống"
                };
            }

            // Tạo mã OTP và lưu vào otpStore
            string otp = GenerateOTP();
            otpStore[email] = (otp, DateTime.Now.AddMinutes(10)); // Lưu mã OTP và thời gian hết hạn trong 10 phút

            // Gửi mã OTP qua email
            await SendOtpEmailAsync(email, otp);

            return new ResultCustomModel<string>
            {
                Code = 200,
                Success = true,
                Message = "Mã OTP đã được gửi đến email của bạn"
            };
        }

        public async Task<ResultCustomModel<string>> ResetPasswordAsync(string otp, string newPassword)
        {
            // Kiểm tra xem OTP có tồn tại và chưa hết hạn không
            var email = otpStore.FirstOrDefault(x => x.Value.otp == otp && x.Value.expiration >= DateTime.Now).Key;

            if (string.IsNullOrEmpty(email))
            {
                return new ResultCustomModel<string>
                {
                    Code = 400,
                    Success = false,
                    Message = "Mã OTP không hợp lệ hoặc đã hết hạn"
                };
            }

            // Tìm khách hàng theo email
            var customer = await _db.Customers.FirstOrDefaultAsync(x => x.Email == email);

            if (customer == null)
            {
                return new ResultCustomModel<string>
                {
                    Code = 400,
                    Success = false,
                    Message = "Không tìm thấy khách hàng tương ứng với mã OTP"
                };
            }

            // Kiểm tra độ dài mật khẩu mới
            if (newPassword.Length <= 5)
            {
                return new ResultCustomModel<string>
                {
                    Code = 400,
                    Success = false,
                    Message = "Mật khẩu phải có độ dài lớn hơn 5 ký tự"
                };
            }

            // Kiểm tra xem mật khẩu mới có trùng với mật khẩu cũ không
            if (newPassword.ToScryptEncode() == customer.Password)
            {
                return new ResultCustomModel<string>
                {
                    Code = 400,
                    Success = false,
                    Message = "Mật khẩu mới không được trùng với mật khẩu cũ"
                };
            }

            // Cập nhật mật khẩu mới và xóa OTP khỏi otpStore
            customer.Password = newPassword.ToScryptEncode(); // Mã hóa mật khẩu mới
            otpStore.Remove(email); // Xóa OTP sau khi sử dụng
            _db.Customers.Update(customer);
            await _db.SaveChangesAsync();

            return new ResultCustomModel<string>
            {
                Code = 200,
                Success = true,
                Message = "Mật khẩu đã được cập nhật thành công"
            };
        }

        public async Task<ResultCustomModel<int>> GetTotalCustomersAsync()
        {
            try
            {
                // Đếm tổng số đơn hàng trong bảng Orders
                int totalCustomers = await _db.Customers.CountAsync();

                return new ResultCustomModel<int>
                {
                    Code = 200,  // Trả về mã trạng thái thành công
                    Data = totalCustomers,
                    Success = true,
                    Message = "Tổng số đơn hàng "
                };
            }
            catch (Exception ex)
            {
                return new ResultCustomModel<int>
                {
                    Code = 500,  // Trả về mã trạng thái lỗi nếu có lỗi
                    Data = 0,
                    Success = false,
                    Message = $"Lỗi khi tính tổng số đơn hàng: {ex.Message}"
                };
            }
        }

        //Up load và lưu ảnh
        public async Task<string> UploadAvatar(int customerId, IFormFile avatar)
        {
            if (avatar == null || avatar.Length == 0)
                throw new Exception("Chưa chọn file ảnh.");

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(avatar.FileName)}";
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "avatars");
            Directory.CreateDirectory(folderPath);
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await avatar.CopyToAsync(stream);
            }

            var customer = await _db.Customers.FindAsync(customerId);
            if (customer == null)
                throw new Exception("Không tìm thấy khách hàng.");

            customer.Image = $"/avatars/{fileName}";
            _db.Customers.Update(customer);
            await _db.SaveChangesAsync();

            return customer.Image;
        }
    }
}




