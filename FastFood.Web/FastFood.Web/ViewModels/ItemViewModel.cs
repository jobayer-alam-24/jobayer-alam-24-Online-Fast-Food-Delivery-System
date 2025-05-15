using FastFood.Models;
using Microsoft.Extensions.FileProviders;
using System.ComponentModel.DataAnnotations;

namespace FastFood.Web.ViewModels
{
    public class ItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int CategoryId { get; set; }
        public string CategoryTitle { get; set; }
        public string SubCategoryTitle { get; set; }
        public int SubCategoryId { get; set; }
        public IFormFile ImageUrl { get; set; }

    }
}
