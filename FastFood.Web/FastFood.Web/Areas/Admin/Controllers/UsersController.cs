using System.Security.Claims;
using FastFood.Models;
using FastFood.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FastFood.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext context;
        public UsersController(ApplicationDbContext context)
        {
            this.context = context;
         
        }
        public IActionResult Index()
        {
            var users = context.ApplicationUsers.ToList();
            if(users != null)
            {
                return View(users);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
