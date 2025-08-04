namespace TreeStore.Models.LoginModels
{
    public class UpdateUserRequest
    {
        public int UserId { get; set; }
        public string Fullname { get; set; }
        public List<int> ListRoles { get; set; }
        public string Phone { get; set; }
        public bool? IsActive { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public DateTime? Birthday { get; set; }
    }
}
