using eShop.Areas.Identity.Data;
using eShop.Data;
using eShop.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace eShop.Controllers
{
    public class OrdersController : Controller
    {
        private readonly eShopContext _context;
        private readonly SignInManager<eShopUser> signinmanager;
        private readonly UserManager<eShopUser> userManager;

        public OrdersController(eShopContext context, SignInManager<eShopUser> signinmanager, UserManager<eShopUser> userManager)
        {
            _context = context;
            this.signinmanager = signinmanager;
            this.userManager = userManager;
        }

        [HttpGet]
        public ActionResult Index(string id)
        {
            IEnumerable<Order> orders;
            if (id == null) 
            {
                if (User.IsInRole(Constants.AdminRole))
                {
                    orders = _context.Orders;     
                }
                else
                {
                    orders = _context.Orders.Where(o => o.Email == userManager.GetUserName(User));
                }
            }
            else 
            {
                orders = _context.Orders.Where(o => o.Customer == id);
            }

            return View(orders);
        }

        [HttpPost]
        public ActionResult Index(IFormCollection formCollection, string id)
        {
            string[] items = formCollection["IDcheckbox"];
            foreach (string item in items)
            {
                var order = _context.Orders.Find(int.Parse(item));
                _context.Orders.Remove(order);
                _context.SaveChanges();
            }
            if (id != null)
            {
                return RedirectToAction(nameof(Index), new { id });
            }
            return RedirectToAction(nameof(Index));
        }

        public ActionResult Details(int id)
        {
            List<OrderDetail> orderDetails = new List<OrderDetail>();
            var orderdetails = _context.OrderDetails.Include(o=>o.Part).Where(o => o.OrderID == id).ToList();
            return View(orderdetails);
        }

        [Authorize(Roles = Constants.AdminRole)]
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var order = _context.Orders.Find(id);
            return View(order);
        }

        [Authorize(Roles = Constants.AdminRole)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Order order)
        {
            _context.Orders.Update(order);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}