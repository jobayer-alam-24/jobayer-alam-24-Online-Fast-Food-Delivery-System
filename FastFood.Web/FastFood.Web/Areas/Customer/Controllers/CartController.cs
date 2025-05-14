using System.Security.Claims;
using FastFood.Models;
using FastFood.Repository;
using FastFood.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastFood.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        [BindProperty]
        public CartOrderViewModel details { get; set; }
        public CartController(ApplicationDbContext context)
        {

            _context = context;
        }
        public IActionResult Index()
        {
            details = new CartOrderViewModel()
            {
                OrderHeader = new OrderHeader()
            };
            var loggedInUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
            details.ListOfCart = _context.Carts.Include(x => x.Item).Where(x => x.ApplicationUserId == loggedInUserId).ToList();
            if(details.ListOfCart != null)
            {
                foreach (var cart in details.ListOfCart)
                {
                   details.OrderHeader.OrderTotal += cart.Item.Price * cart.Count;
                }
            }
            return View(details);
        }
        [HttpGet]
        public IActionResult Summary()
        {
            var loggedInUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
           details = new CartOrderViewModel()
           {
               OrderHeader = new OrderHeader(),
               ListOfCart = _context.Carts.Include(x => x.Item).Where(x => x.ApplicationUserId == loggedInUserId).ToList()
           };
            details.OrderHeader.ApplicationUser = _context.ApplicationUsers.FirstOrDefault(x => x.Id == loggedInUserId);
            details.OrderHeader.Name = details.OrderHeader.ApplicationUser.Name;
            details.OrderHeader.PhoneNumber = details.OrderHeader.ApplicationUser.PhoneNumber;
            foreach(var cart in details.ListOfCart)
            {
                details.OrderHeader.OrderTotal += cart.Item.Price * cart.Count;
            }
            return View(details);
        }
        [HttpPost]
        public IActionResult Summary(CartOrderViewModel vm)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.ApplicationUsers.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = "User Not Found";
                return View(vm);
            }

            
            vm.ListOfCart = _context.Carts
                .Where(c => c.ApplicationUserId == userId)
                .Include(c => c.Item)
                .ToList();

            if (vm.ListOfCart == null || !vm.ListOfCart.Any())
            {
                ViewBag.ErrorMessage = "Cart is empty";
                return View(vm);
            }

            var newOrder = new OrderHeader
            {
                ApplicationUserId = user.Id,
                ApplicationUser = user,
                Name = vm.OrderHeader.Name,
                PhoneNumber = vm.OrderHeader.PhoneNumber,
                OrderDate = DateTime.Now,
                TimeOfPick = vm.OrderHeader.TimeOfPick,
                DateOfPick = vm.OrderHeader.DateOfPick,
                OrderStatus = OrderStatus.Pending,
                PaymentStatus = PaymentStatus.Pending,
                TransactionId = Guid.NewGuid().ToString()
            };

            foreach (var cart in vm.ListOfCart)
            {
                if (cart.Item == null)
                {
                    ViewBag.ErrorMessage = "One or more items in the cart are invalid.";
                    return View(vm);
                }

                newOrder.SubTotal += cart.Item.Price * cart.Count;
            }

            newOrder.OrderTotal = newOrder.SubTotal - newOrder.CouponDiscount;

            _context.OrderHeaders.Add(newOrder);
            _context.SaveChanges();
            foreach (var cart in vm.ListOfCart)
            {
                var orderDetails = new OrderDetails
                {
                    OrderHeaderId = newOrder.Id,
                    ItemId = cart.Item.Id,
                    Item = cart.Item,
                    Count = cart.Count,
                    Name = cart.Item.Title,
                    Description = cart.Item.Description,
                    Price = cart.Item.Price
                };

                _context.OrderDetails.Add(orderDetails);
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Increase(int id)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(x => x.Id == id);
            if (cart != null)
            {
                cart.Count += 1;
                HttpContext.Session.SetInt32("SessionCart", cart.Count);
                _context.Carts.Update(cart);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Decrease(int id)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(x => x.Id == id);
            if (cart != null)
            {
                if(cart.Count > 1)
                {
                    cart.Count -= 1;
                    HttpContext.Session.SetInt32("SessionCart", cart.Count);
                    _context.Carts.Update(cart);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Remove(int id)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(x => x.Id == id);
            if (cart != null)
            {
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();
                var cartItems = await _context.Carts
           .Where(x => x.ApplicationUserId == cart.ApplicationUserId)
           .ToListAsync();

                HttpContext.Session.SetInt32("SessionCart", cartItems.Count);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
