namespace TreeStore.Models.UserModels
{
    public class UserResponse
    {
        public int UserId { get; set; }

        public string Username { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public DateTime? Birthday { get; set; }

        public bool? IsActive { get; set; }

        public DateTime CreateOn { get; set; }

        public string Fullname { get; set; }

        public string Avatar { get; set; }

        public string Position { get; set; }
        public List<int> lstRolesId { get; set; }
    }
}
