using TreeStore.Models.Entities;

namespace TreeStore.Models.Order
{
    public class DetailOrderReponse
    {
        public string NameCustomer { get; set; }
        public string Address { get; set; }
        public short StateId { get; set; }


        public List<GetDetailProductOrderSPResult> DetailProducts { get; set; }
    }
}
