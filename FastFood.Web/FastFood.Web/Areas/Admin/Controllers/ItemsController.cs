using System.Threading.Tasks;
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
    public class ItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ItemsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _webHostEnvironment = environment;
            _context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var listFromDb = _context.Items.Include(x => x.Category).Include(x => x.SubCategory).Select(x => new ItemViewModel()
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                Price = x.Price,
                CategoryId = x.CategoryId,
                SubCategoryId = x.SubCategoryId
            }).ToList();

            return View(listFromDb);
        }
        [HttpGet]
        public IActionResult Create()
        {
            var model = new ItemViewModel();
            ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Title");
            return View(model);
        }
        [HttpGet]
        public IActionResult GetSubCategory(int categoryId)
        {
            var subCategories = _context.SubCategories
       .Where(x => x.CategoryId == categoryId)
       .Select(x => new { id = x.Id, title = x.Title })
       .ToList();
            return Json(subCategories);
        }
        [HttpPost]
        public async Task<IActionResult> Create(ItemViewModel vm)
        {
            Item model = new Item();
            if (ModelState.IsValid)
           {
                if(vm.ImageUrl != null && vm.ImageUrl.Length > 0)
                {
                    var uploadDir = @"Images/Items";
                    var fileName = Guid.NewGuid().ToString() + "-" + vm.ImageUrl.FileName;
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath,uploadDir, fileName);
                    await vm.ImageUrl.CopyToAsync(new FileStream(filePath, FileMode.Create));
                    model.Image = "/" + uploadDir + "/" + fileName;
                }
                model.Title = vm.Title;
                model.Description = vm.Description;
                model.Price = vm.Price;
                model.CategoryId = vm.CategoryId;
                model.SubCategoryId = vm.SubCategoryId;
                _context.Items.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
           return View(vm);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var item = _context.Items.FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            var model = new ItemViewModel
            {
                Id = item.Id,
                Title = item.Title,
                Description = item.Description,
                Price = item.Price,
                CategoryId = item.CategoryId,
                SubCategoryId = item.SubCategoryId
            };

            ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Title");
            ViewBag.SubCategories = new SelectList(_context.SubCategories.Where(x => x.CategoryId == item.CategoryId).ToList(), "Id", "Title");

            ViewBag.CurrentImage = item.Image;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ItemViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Title");
                ViewBag.SubCategories = new SelectList(_context.SubCategories.Where(x => x.CategoryId == vm.CategoryId).ToList(), "Id", "Title");
                return View(vm);
            }

            var itemFromDb = _context.Items.FirstOrDefault(x => x.Id == vm.Id);
            if (itemFromDb == null)
            {
                return NotFound();
            }

            if (vm.ImageUrl != null && vm.ImageUrl.Length > 0)
            {
                var uploadDir = @"Images/Items";
                var fileName = Guid.NewGuid().ToString() + "-" + vm.ImageUrl.FileName;
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, uploadDir, fileName);

                if (!string.IsNullOrEmpty(itemFromDb.Image))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, itemFromDb.Image.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                await vm.ImageUrl.CopyToAsync(new FileStream(filePath, FileMode.Create));
                itemFromDb.Image = "/" + uploadDir + "/" + fileName;
            }

            itemFromDb.Title = vm.Title;
            itemFromDb.Description = vm.Description;
            itemFromDb.Price = vm.Price;
            itemFromDb.CategoryId = vm.CategoryId;
            itemFromDb.SubCategoryId = vm.SubCategoryId;

            _context.Items.Update(itemFromDb);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var itemFromDb = _context.Items.FirstOrDefault(x => x.Id == id);
            if (itemFromDb == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(itemFromDb.Image))
            {
                var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, itemFromDb.Image.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.Items.Remove(itemFromDb);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}
