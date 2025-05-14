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

            if (User.IsInRole("Admin"))
            {
                orders = _context.OrderHeaders
                    .Include(x => x.ApplicationUser)
                    .ToList();
            }
            else
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                orders = _context.OrderHeaders
                    .Include(x => x.ApplicationUser)
                    .Where(x => x.ApplicationUserId == userId)
                    .ToList();
            }

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<OrderStatus>(status, true, out var parsedStatus))
            {
                orders = orders.Where(x => x.OrderStatus == parsedStatus).ToList();
            }

            return View(orders);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var orderHeader = await _context.OrderHeaders.FindAsync(id);
            ViewBag.Total = orderHeader.OrderTotal;
            if (orderHeader == null) return NotFound();

            return View(orderHeader);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OrderHeader orderHeader)
        {
            if (id != orderHeader.Id) return NotFound();
            ViewData["Total"] = orderHeader.OrderTotal;
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderHeader);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.OrderHeaders.Any(e => e.Id == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(orderHeader);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var orderHeader = await _context.OrderHeaders.FindAsync(id);
            if (orderHeader != null)
            {
                _context.OrderHeaders.Remove(orderHeader);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
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
