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
