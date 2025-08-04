namespace TreeStore.Models.LoginModels
{
    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public DateTime? Birthday { get; set; }
        public string Fullname { get; set; }
        public string Position { get; set; }
    }
}
