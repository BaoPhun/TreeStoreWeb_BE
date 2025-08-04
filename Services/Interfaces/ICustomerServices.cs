using TreeStore.Models.CustomerModels;
using TreeStore.Models.CustomModels;
using TreeStore.Models.Entities;

namespace TreeStore.Services.Interfaces
{
    public interface ICustomerServices
    {
        Task ActivateAccountAsync(int customerId);
        Task DeactivateAccountAsync(int customerId);
        Task<ResultCustomModel<bool>> ChangeActiveAsync(int customerId);
        Task<ResultCustomModel<List<CustomerResponse>>> ListCustomersAsync();

        Task<ResultCustomModel<CustomerResponse>> GetCustomerByIdAsync(int customerId);
        Task<ResultCustomModel<int>> GetCurrentCustomerIdAsync(string email);
        Task<ResultCustomModel<CustomerResponse>> UpdateCustomer(UpdateCustomerRequest request);
        Task<ResultCustomModel<List<CustomerResponse>>> SearchCustomerByNameAsync(string name);
        Task<ResultCustomModel<CustomerResponse>> RegisterCustomerAsync(CustomerRequest customerRequest);

        Task<ResultCustomModel<CustomerResponse>> LoginAsync(LoginCustomer loginCustomer);
        Task<ResultCustomModel<string>> LogoutAsync();
        Task<ResultCustomModel<string>> ForgotPasswordAsync(string email);
        Task<ResultCustomModel<string>> ResetPasswordAsync(string email, string otp);
        Task<ResultCustomModel<int>> GetTotalCustomersAsync();
        Task<string> UploadAvatar(int customerId, IFormFile avatar);

    }
}
