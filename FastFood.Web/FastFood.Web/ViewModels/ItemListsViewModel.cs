using FastFood.Models;
using Microsoft.Extensions.FileProviders;
using System.ComponentModel.DataAnnotations;

namespace FastFood.Web.ViewModels
{
    public class ItemListsViewModel
    {
        public ICollection<Item> Items { get; set; } = new List<Item>();
        public ICollection<Category> Categories { get; set; } = new List<Category>();
        public ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();
    }
}
