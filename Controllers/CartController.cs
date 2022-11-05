using System.Collections.Generic;
using System.Linq;
using eShop.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using eShop.Models.Database;
using System.Text;
using System;
using Microsoft.AspNetCore.Identity;
using eShop.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace eShop.Controllers
{
    public class CartController : Controller
    {
        public string ShoppingCartId { get; set; }
        private readonly eShopContext _db;
        private readonly SignInManager<eShopUser> signinmanager;
        private readonly UserManager<eShopUser> userManager;
        public CartController(eShopContext db, SignInManager<eShopUser> signinmanager, UserManager<eShopUser> userManager)
        {
            _db = db;
            this.signinmanager = signinmanager;
            this.userManager = userManager;
        }

        public ActionResult Index()
        {
            ShoppingCartId = GetCartId();
            IEnumerable <CartItem> cartitem = _db.ShoppingCartItems.Include(c=>c.Part).Where(c => c.CartId == ShoppingCartId).ToList();
            return View(cartitem);
        }

        public IActionResult Remove(int id)
        {
            ShoppingCartId = GetCartId();
            var cartitem = _db.ShoppingCartItems.Where(c => c.CartId == ShoppingCartId && c.PartId == id).FirstOrDefault();
            _db.ShoppingCartItems.Remove(cartitem);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Increase(int id)
        {
            ShoppingCartId = GetCartId();
            var cartitem = _db.ShoppingCartItems.Where(c => c.CartId == ShoppingCartId && c.PartId == id).FirstOrDefault();
            cartitem.Quantity++;
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Decrease(int id)
        {
            ShoppingCartId = GetCartId();
            var cartitem = _db.ShoppingCartItems.Where(c => c.CartId == ShoppingCartId && c.PartId == id).FirstOrDefault();
            if (cartitem.Quantity != 1) 
            {
                cartitem.Quantity--;
            }
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public string GetCartId()
        {
            if (HttpContext.Session.GetString(Constants.SessionCart) == null)
            {
                if (signinmanager.IsSignedIn(User))
                {
                    HttpContext.Session.SetString(Constants.SessionCart, userManager.GetUserName(User));
                }
                else
                { 
                    HttpContext.Session.SetString(Constants.SessionCart, Guid.NewGuid().ToString());
                }
            }
            return HttpContext.Session.GetString(Constants.SessionCart).ToString();
        }
    }
}