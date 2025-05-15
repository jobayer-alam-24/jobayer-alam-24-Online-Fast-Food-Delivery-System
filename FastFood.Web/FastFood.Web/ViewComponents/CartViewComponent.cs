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
                var sessionKey = $"SessionCart_{userId}";
                var sessionCount = HttpContext.Session.GetInt32(sessionKey);

                if (sessionCount.HasValue)
                {
                    return View(sessionCount.Value);
                }
                else
                {
                    var cartCount = await _context.Carts
                        .Include(x => x.Item)
                        .Where(x => x.ApplicationUserId == userId)
                        .SumAsync(x => x.Count);

                    HttpContext.Session.SetInt32(sessionKey, cartCount);
                    return View(cartCount);
                }
            }

            return View(0);
        }


    }
}
