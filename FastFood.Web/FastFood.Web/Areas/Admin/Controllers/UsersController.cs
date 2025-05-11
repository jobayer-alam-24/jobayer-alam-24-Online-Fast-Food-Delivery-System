using System.Security.Claims;
using FastFood.Repository;
using Microsoft.AspNetCore.Mvc;

namespace FastFood.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext context;
        public UsersController(ApplicationDbContext context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return View(context.ApplicationUsers.Where(x => x.Id == claims).ToList());
        }
    }
}
