namespace TreeStore.Models.LoginModels
{
    public class LoginResponse
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string RoleName { get; set; }
        public string[] Role { get; set; }
        public string Avatar { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Token { get; set; }
        public DateTime? Birthday { get; set; }
        public DateTime CreateOn { get; set; }
    }
}
