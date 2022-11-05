using eShop.Areas.Identity.Data;
using eShop.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: HostingStartup(typeof(eShop.Areas.Identity.IdentityHostingStartup))]
namespace eShop.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<eShopContext>(options =>
                options.UseSqlServer(
                context.Configuration.GetConnectionString("eShopContextConnection")));
                services.AddIdentity<eShopUser, IdentityRole>(options =>options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<eShopContext>();
            });
        }
    }
}