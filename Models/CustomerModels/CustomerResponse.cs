namespace TreeStore.Models.CustomerModels;
using TreeStore.Models.Entities;

public class CustomerResponse
{
    /// <summary>
    ///   
    public int CustomerId { get; set; }

    public string Fullname { get; set; }
    public string Image { get; set; }
    //public string ListRole { get; set; }
    public bool? IsActive { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }

    /// </summary>

}