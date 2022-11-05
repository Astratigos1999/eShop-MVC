using eShop.Areas.Identity.Data;
using eShop.Models.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Type = eShop.Models.Database.Type;

namespace eShop.Data
{
    public class eShopContext : IdentityDbContext<eShopUser>
    {
        public eShopContext(DbContextOptions<eShopContext> options)
            : base(options)
        {
        }
        public DbSet<Part> Parts { get; set;  }
        public DbSet<Type> Types { get; set; }
        public override DbSet<eShopUser> Users { get; set; }
        public DbSet<CartItem> ShoppingCartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<eShopUser>(b =>
            {
                b.ToTable("Users");
            });
            builder.Entity<IdentityUserClaim<string>>(b => 
            {
                b.ToTable("Claims");
            });
            builder.Entity<IdentityUserLogin<string>>(b =>
            {
                b.ToTable("Logins");
            });
            builder.Entity<IdentityUserToken<string>>(b =>
            {
                b.ToTable("Tokens");
            });
            builder.Entity<IdentityRole>(b =>
            {
                b.ToTable("Roles");
            });
            builder.Entity<IdentityRoleClaim<string>>(b =>
            {
                b.ToTable("RoleClaims");
            });
            builder.Entity<IdentityUserRole<string>>(b =>
            {
                b.ToTable("UserRoles");
            });
        }
    }
}
