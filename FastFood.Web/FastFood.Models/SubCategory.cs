using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastFood.Models
{
    public class SubCategory
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is Required.")]
        public string Title { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public ICollection<Item> Items { get; set; }
    }
}
