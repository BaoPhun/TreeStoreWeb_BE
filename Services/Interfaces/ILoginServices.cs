using TreeStore.Models.CustomModels;
using TreeStore.Models.CustomModels;
using TreeStore.Models.Entities;
using TreeStore.Models.LoginModels;

namespace TreeStore.Services.Interfaces
{
    public interface ILoginServices
    {
        Task<ResultCustomModel<RegisterResponse>> RegisterAsync(RegisterRequest registerRequest);
        Task<ResultCustomModel<LoginResponse>> SignInAsync(LoginRequest login);

    }
}
