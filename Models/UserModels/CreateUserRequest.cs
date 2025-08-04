namespace TreeStore.Models.LoginModels
{
    public class CreateUserRequest
    {
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string IsActive { get; set; }
        public List<int> ListRoles { get; set; }
        public string? Password { get; set; }
        public string? Avatar { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public DateTime? Birthday { get; set; }
    }
}
