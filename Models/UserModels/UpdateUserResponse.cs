using System.Net;
using System.Numerics;
using TreeStore.Models.Entities;

namespace TreeStore.Models.UserModels
{
    public class UpdateUserResponse
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string Avatar { get; set; }
        public bool? IsActive { get; set; }
        public string Position { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public DateTime? Birthday { get; set; }


    }
}

