using FastFood.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FastFood.Repository
    {
        public class ApplicationDbContext : IdentityDbContext
        {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                : base(options)
            {
            }

            public DbSet<ApplicationUser> ApplicationUsers { get; set; }

            public DbSet<Category> Categories { get; set; }
            public DbSet<SubCategory> SubCategories { get; set; }

            public DbSet<Item> Items { get; set; }

            public DbSet<Coupon> Coupons { get; set; }

            public DbSet<Cart> Carts { get; set; }

            public DbSet<OrderHeader> OrderHeaders { get; set; }
            public DbSet<OrderDetails> OrderDetails { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                modelBuilder.Entity<Item>()
                    .HasOne(i => i.Category)
                    .WithMany()
                    .HasForeignKey(i => i.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade); 

                modelBuilder.Entity<Item>()
                    .HasOne(i => i.SubCategory)
                    .WithMany()
                    .HasForeignKey(i => i.SubCategoryId)
                    .OnDelete(DeleteBehavior.Restrict); 

                
            }
        }
    }
