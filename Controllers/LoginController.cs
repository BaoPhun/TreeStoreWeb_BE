using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TreeStore.Models.CustomModels;
using TreeStore.Models.Entities;
using TreeStore.Models.LoginModels;
using TreeStore.Services.Interfaces;
using TreeStore.Utilities;

namespace TreeStore.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly ILoginServices _loginServices;

        public LoginController(ILoginServices loginServices)
        {
            _loginServices = loginServices;
        }

        [Route("/Login")]
        [HttpPost]
        public async Task<ResultCustomModel<LoginResponse>> Login([FromBody] LoginRequest loginRequest)
        {
            var loginResult = await _loginServices.SignInAsync(loginRequest);
            return loginResult; 
        }

        [Route("/Register")]
        [HttpPost]
        public async Task<ResultCustomModel<RegisterResponse>> Register([FromBody] RegisterRequest registerRequest)
        {
            var registerResult = await _loginServices.RegisterAsync(registerRequest);
            return registerResult;
        }

    }
}
