using FastFood.Models;

namespace FastFood.Web.ViewModels
{
    public class CartOrderViewModel
    {
        public List<Cart> ListOfCart { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}
