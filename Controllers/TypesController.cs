using System.Collections.Generic;
using eShop.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eShop.Controllers
{
    [Authorize(Roles = Constants.AdminRole)]// Μόνο ο διαχειριστής έχει την εξουσιοδώτηση για αυτόν τον Controller
    public class TypesController : Controller
    {
        private readonly eShopContext _context;

        public TypesController(eShopContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            IEnumerable<Models.Database.Type> objList = _context.Types;
            return View(objList);
        }

        [HttpPost]
        public ActionResult Index(IFormCollection formCollection)
        {
            string[] items = formCollection["IDcheckbox"];// πίνακας επιλεγμένων δεδομένων στη φόρμα
            foreach (string item in items)
            {
                var types = _context.Types.Find(int.Parse(item));
                _context.Types.Remove(types);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Models.Database.Type type)
        {
            if (ModelState.IsValid)
            {
                _context.Types.Add(type);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(type);
        }
    }
}