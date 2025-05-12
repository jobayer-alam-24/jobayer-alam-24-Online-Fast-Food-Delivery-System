using System.Diagnostics;
using System.Security.Claims;
using FastFood.Models;
using FastFood.Repository;
using FastFood.Web.Models;
using FastFood.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastFood.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context) 
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ItemListsViewModel vm = new ItemListsViewModel()
            {
                Items = await _context.Items.Include(x => x.Category).Include(x => x.SubCategory).ToListAsync(),
                Categories = await _context.Categories.ToListAsync(),
                Coupons = await _context.Coupons.Where(x => x.IsActive == true).ToListAsync()
            };
            if (vm != null)
            {
                return View(vm);
            }
            else
            {
                return RedirectToAction("Index","Items");
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var items = await _context.Items.Include(x => x.Category).Include(x => x.SubCategory).FirstOrDefaultAsync(x => x.Id == id);
            var cart = new Cart()
            {
                ItemId = items.Id,
                Item = items,
                Count = 1
            };
            return View(cart);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Details(Cart cart)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/Identity/Account/Register");
            }
            if (ModelState.IsValid)
            {
                cart.ApplicationUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var cartFromDb = await _context.Carts.FirstOrDefaultAsync(x => x.ApplicationUserId == cart.ApplicationUserId && x.ItemId == cart.ItemId);
                if (cartFromDb == null)
                {
                    cart.Id = 0;
                    await _context.Carts.AddAsync(cart);
                }
                else
                {
                    cartFromDb.Count += cart.Count;
                    _context.Carts.Update(cartFromDb);
                }
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
