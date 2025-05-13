using FastFood.Models;
using FastFood.Repository;
using FastFood.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastFood.Web.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(string status)
        {
            IEnumerable<OrderHeader> orders;
            if(User.IsInRole("Admin"))
            {
                orders = _context.OrderHeaders.Include(x => x.ApplicationUser).ToList();
            }
            else
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                orders = _context.OrderHeaders.Include(x => x.ApplicationUser).Where(x => x.ApplicationUserId == userId).ToList();
                switch (status)
                {
                    case "pending":
                        orders = orders.Where(x => x.OrderStatus == "Pending".ToLower()).ToList();
                        break;
                    case "approved":
                        orders = orders.Where(x => x.OrderStatus == "Approved".ToLower()).ToList();
                        break;
                    case "underprocess":
                        orders = orders.Where(x => x.OrderStatus == "UnderProcess".ToLower()).ToList();
                        break;
                    case "shipped":
                        orders = orders.Where(x => x.OrderStatus == "Shipped".ToLower()).ToList();
                        break;
                    default:
                        break;
                }
            }
            return View(orders);
        }
        public IActionResult OrderDetails(int id)
        {
            var orderDetails = new OrderDetailsViewModel()
            {
                OrderHeader = _context.OrderHeaders.Include(x => x.ApplicationUser).FirstOrDefault(x => x.Id == id),
                OrderDetails = _context.OrderDetails.Include(x => x.Item).Where(x => x.OrderHeaderId == id).ToList()
            };
            return View(orderDetails);
        }
    }
}
