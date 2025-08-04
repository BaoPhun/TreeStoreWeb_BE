using System.ComponentModel.DataAnnotations;

namespace TreeStore.Models.LoginModels
{
    public class LoginRequest
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
