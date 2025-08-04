using TreeStore.Models.CustomModels;
using TreeStore.Models.Entities;
using TreeStore.Models.LoginModels;
using TreeStore.Models.UserModels;
using TreeStore.Models.CustomerModels;

namespace TreeStore.Services.Interfaces
{
    public interface IUserServices
    {
        Task ActivateAccountAsync(int userId);
        Task DeactivateAccountAsync(int userId);
        Task<ResultCustomModel<List<UpdateUserResponse>>> ListUsersAsync();

        Task<ResultCustomModel<UpdateUserResponse>> UpdateUserAsync(UpdateUserRequest updateUserRequest);
        Task<ResultCustomModel<bool>> CreateAsync(CreateUserRequest request);
        Task<ResultCustomModel<UserResponse>> GetUserByIdAsync(int userId);
        Task<ResultCustomModel<bool>> ChangeActiveAsync(int userId);
        Task<ResultCustomModel<List<Role>>> GetRolesAsync();
        Task<List<UserResponse>> SearchUserByNameAsync(string name);

    }
}
