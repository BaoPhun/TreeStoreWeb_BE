namespace TreeStore.Models.CustomerModels
{
    public class UpdateCustomerRequest
    {

        /// <summary>
        ///  
        public int CustomerId { get; set; }

        public string Fullname { get; set; }
     
    
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string? Password { get; set; }
        /// </summary>

    }
}
