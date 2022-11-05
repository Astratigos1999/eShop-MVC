using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using eShop.Areas.Identity.Data;
using eShop.Data;
using eShop.Models.Database;
using eShop.Models.ViewModels;
using eShop.Utility;
using Microsoft.EntityFrameworkCore;

namespace eShop.Controllers
{
    public class CheckoutController : Controller
    {
        public string ShoppingCartId { get; set; }
        private readonly eShopContext _context;
        private readonly SignInManager<eShopUser> signinmanager;
        private readonly UserManager<eShopUser> userManager;

        public CheckoutController(eShopContext context, SignInManager<eShopUser> signinmanager, UserManager<eShopUser> userManager)
        {
            _context = context;
            this.signinmanager = signinmanager;
            this.userManager = userManager;
        }

        public ActionResult Index()
        {   
            ShoppingCartId = GetCartId();
            int total = 0;
            total = _context.ShoppingCartItems.Where(i => i.CartId == ShoppingCartId).Select(t => t.Quantity * t.Part.Price).Sum();
            HttpContext.Session.SetInt32("Total", total);
            return View();
        }

        public ActionResult Success()
        {
            ShoppingCartId = GetCartId();
            IEnumerable<CartItem> cartitems = _context.ShoppingCartItems.Include(c => c.Part).Where(c => c.CartId == ShoppingCartId).ToList();

            Order order = new Order();
            if (signinmanager.IsSignedIn(User))
            {
                var user = _context.Users.Find(userManager.GetUserId(User));
                order.FirstName = user.Firstname;
                order.LastName = user.Lastname;
                order.Email = user.Email;
                order.Phone = user.PhoneNumber;
                order.City = user.City;
                order.Address = user.Address;
                order.PostalCode = user.ZIP;
            }
            else
            {
                Credentials user = HttpContext.Session.Get<Credentials>(Constants.Credentials);
                order.FirstName = user.Firstname;
                order.LastName = user.Lastname;
                order.Email = user.Email;
                order.Phone = user.Phone.ToString();
                order.City = user.City;
                order.Address = user.Address;
                order.PostalCode = user.ZIP;
            }
            order.Customer = SetCustomer();
            order.OrderDate = DateTime.Now;
            order.HasBeenShipped = false;
            order.Total = (int)HttpContext.Session.GetInt32("Total");
            ViewBag.Customer = order.Customer;
            _context.Orders.Add(order);
            _context.SaveChanges();

            foreach (var cartitem in cartitems)
            {
                var orderdetail = new OrderDetail();
                orderdetail.OrderID = order.OrderId;
                orderdetail.Customer = order.Customer;
                orderdetail.PartID = cartitem.PartId;
                orderdetail.Quantity = cartitem.Quantity;
                orderdetail.UnitPrice = cartitem.Part.Price;
                _context.OrderDetails.Add(orderdetail);
                _context.SaveChanges();
            }

            foreach (var cartitem in cartitems) 
            {
                _context.ShoppingCartItems.Remove(cartitem);
                _context.SaveChanges();
            }

            HttpContext.Session.Set("Total", string.Empty);

            return View("Success");
        }

        [HttpGet]
        public ActionResult Credentials()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Credentials(Credentials credentials)
        {
            HttpContext.Session.Set<Credentials>(Constants.Credentials, credentials);
            return RedirectToAction("Index");
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
            return HttpContext.Session.GetString(Constants.SessionCart);
        }

        public string SetCustomer() 
        {
            string customer;
            if (signinmanager.IsSignedIn(this.User))
            {
                customer = userManager.GetUserName(this.User);
            }
            else
            {
                customer = Guid.NewGuid().ToString();
            }
            return customer;
        }
    }
}
