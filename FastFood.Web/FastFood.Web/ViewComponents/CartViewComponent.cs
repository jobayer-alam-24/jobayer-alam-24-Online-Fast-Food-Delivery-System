using System.Security.Claims;
using FastFood.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastFood.Web.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public CartViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userIdentity = User.Identity as ClaimsIdentity;
            var userId = userIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                var sessionCount = HttpContext.Session.GetInt32("SessionCart");

                if (sessionCount.HasValue)
                {
                    return View(sessionCount.Value); 
                }
                else
                {
                    var cart = await _context.Carts
                        .Include(x => x.Item)
                        .Where(x => x.ApplicationUserId == userId)
                        .ToListAsync();

                    int itemCount = cart?.Count ?? 0;

                    HttpContext.Session.SetInt32("SessionCart", itemCount);
                    return View(itemCount);
                }
            }

            return View(0);
        }

    }
}
