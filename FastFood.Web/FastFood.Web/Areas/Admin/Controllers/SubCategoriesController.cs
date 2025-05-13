using FastFood.Models;
using FastFood.Repository;
using FastFood.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FastFood.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SubCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public SubCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var listFromDb = _context.SubCategories.Include(x => x.Category).ToList();

            return View(listFromDb);
        }
        [HttpGet]
        public IActionResult Create()
        {
            SubCategoryViewModel vm = new SubCategoryViewModel();
            ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Title");
            return View(vm);
        }
        [HttpPost]
        public IActionResult Create(SubCategoryViewModel vm)
        {
            var model = new SubCategory();
            model.Title = vm.Title;
            model.CategoryId = vm.CategoryId;
            if (ModelState.IsValid)
            {
                _context.SubCategories.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(vm);

        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var vm = new SubCategoryViewModel();
            var subCategoryFromDb = _context.SubCategories.FirstOrDefault(x => x.Id == id);
            if(subCategoryFromDb != null)
            {
                vm.Id = subCategoryFromDb.Id;
                vm.Title = subCategoryFromDb.Title;
                ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Title", subCategoryFromDb.CategoryId);
            }
            return View(vm);
        }
        [HttpPost]
        public IActionResult Edit(SubCategoryViewModel vm)
        {
           if(ModelState.IsValid)
            {
                var subcategoryFromDb = _context.SubCategories.FirstOrDefault(x => x.Id == vm.Id);
                if(subcategoryFromDb != null)
                {
                    subcategoryFromDb.Title = vm.Title;
                    _context.SubCategories.Update(subcategoryFromDb);
                    _context.SaveChanges();
                }
                return RedirectToAction("Index");
                
            }
            return View(vm);
          
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var subcategoryFromDb = _context.SubCategories.FirstOrDefault(x => x.Id == id);
            if (subcategoryFromDb != null)
            {
                _context.SubCategories.Remove(subcategoryFromDb);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
