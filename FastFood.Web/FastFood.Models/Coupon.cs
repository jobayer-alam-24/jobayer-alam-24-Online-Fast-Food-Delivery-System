using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastFood.Models
{
    public class Coupon
    {
        public int Id { get; set; }
        public string Title { get; set; }
        [DisplayName("Coupon Type")]
        public CouponType CouponType { get; set; }
        public double Discount { get; set; }
        [DisplayName("Minimum Order Amount")]
        public double MinimumAmount { get; set; }
        [DisplayName("Coupon Picture")]
        public byte[] CouponPicture { get; set; }
        public bool IsActive { get; set; }
    }
    public enum CouponType
    {
        Percentage,
        Currency
    }
}
