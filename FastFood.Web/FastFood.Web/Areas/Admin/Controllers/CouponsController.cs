using FastFood.Models;
using FastFood.Repository;
using Microsoft.AspNetCore.Mvc;
using System.IO;
namespace FastFood.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CouponsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CouponsController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var coupons = _context.Coupons.ToList();
            return View(coupons);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Coupon coupon)
        {
            if(ModelState.IsValid)
            {
                byte[] photo = null;
                var files = Request.Form.Files;
                using (var fileStream = files[0].OpenReadStream())
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        fileStream.CopyTo(memoryStream);
                        photo = memoryStream.ToArray();
                    }
                }
                coupon.CouponPicture = photo;
                _context.Coupons.Add(coupon);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var coupon = _context.Coupons.FirstOrDefault(x => x.Id == id);
            if (coupon == null)
            {
                return NotFound();
            }

            return View(coupon);
        }
        [HttpPost]
        public IActionResult Edit(Coupon coupon)
        {
            if (!ModelState.IsValid)
            {
                return View(coupon);
            }

            var couponFromDb = _context.Coupons.FirstOrDefault(x => x.Id == coupon.Id);
            if (couponFromDb == null)
            {
                return NotFound();
            }
            var files = Request.Form.Files;
            if (files.Count > 0)
            {
                byte[] photo = null;
                using (var fileStream = files[0].OpenReadStream())
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        fileStream.CopyTo(memoryStream);
                        photo = memoryStream.ToArray();
                    }
                }
                couponFromDb.CouponPicture = photo;
            }

            couponFromDb.Title = coupon.Title;
            couponFromDb.CouponType = coupon.CouponType;
            couponFromDb.Discount = coupon.Discount;
            couponFromDb.MinimumAmount = coupon.MinimumAmount;
            couponFromDb.IsActive = coupon.IsActive;

            _context.Coupons.Update(couponFromDb);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var couponFromDb = _context.Coupons.FirstOrDefault(x => x.Id == id);
            if (couponFromDb == null)
            {
                return NotFound();
            }

            _context.Coupons.Remove(couponFromDb);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
