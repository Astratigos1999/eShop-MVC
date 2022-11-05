using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using eShop.Data;
using eShop.Models.Database;
using eShop.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using eShop.Areas.Identity.Data;
using X.PagedList;

namespace eShop.Controllers
{
    public class PartsController : Controller
    {
        public string ShoppingCartId { get; set; }
        private readonly eShopContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly SignInManager<eShopUser> signinmanager;
        private readonly UserManager<eShopUser> userManager;
        public PartsController(eShopContext context, IWebHostEnvironment hostEnvironment, SignInManager<eShopUser> signinmanager, UserManager<eShopUser> userManager)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            this.signinmanager = signinmanager;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult List(string filter, int page = 1)//σελίδα 1 στη σελιδοποιημένη λίστα από προεπιλογή
        {
            List<Part> partslist = new List<Part>();
            if (filter != null)//εάν υπάρχει φίλτρο
            {
                ViewBag.CurrentFilter = filter;
                //εάν το φίλτρο περιέχεται στη περιγραφή, στο όνομα, στον τύπο ή στην κατηγορία οχήματος
                var parts = _context.Parts.Include(p => p.Type)
                    .Where(p => p.Description.Contains(filter) || p.Name.Contains(filter) || p.Type.Name.Contains(filter) || p.VehicleCategory.Contains(filter)).OrderBy(p => p.Price).ToList();
                foreach (var part in parts)
                {
                    partslist.Add(new Part { ID = part.ID, Image = part.Image, Name = part.Name, Price = part.Price });
                }
            }
            else
            {
                var parts = _context.Parts.OrderBy(p => p.Price).ToList();
                foreach (var part in parts)
                {
                    partslist.Add(new Part { ID = part.ID, Image = part.Image, Name = part.Name, Price = part.Price });
                }
            }
            return View(partslist.ToPagedList(page, 5));//επιστροφή σελιδοποιημένης λίστας στο νούμερο σελίδας page, με πεντε προϊόντα ανά σελίδα στη σελίδα List
        }

        [HttpPost]
        public IActionResult List(IFormCollection formCollection, string search)
        {
            string[] items = formCollection["IDcheckbox"];// πίνακας επιλεγμένων δεδομένων στη φόρμα
            foreach (string item in items)
            {
                var product = _context.Parts.Find(int.Parse(item));
                string upload = _hostEnvironment.WebRootPath + Constants.ImagePath;//μονοπάτι φακέλου φωτογραφιών
                var oldFile = Path.Combine(upload, product.Image);//μονοπάτι παλιάς φωτογραφίας
                if (System.IO.File.Exists(oldFile))//εάν υπάρχει η φωτογραφία
                {
                    System.IO.File.Delete(oldFile);//διαγραφή παλιάς φωτογραφίας
                }
                _context.Parts.Remove(product);
                _context.SaveChanges();
            }
            if (search != null) //εάν υπάρχει φίλτρο αναζήτησης
            {
                ViewBag.CurrentFilter = search;//αποθήκευση φίλτρου σε ViewBag
                return RedirectToAction(nameof(List), new { filter = ViewBag.CurrentFilter });//δρομολόγηση στη λίστα με το φίλτρο
            }
            return RedirectToAction(nameof(List));
        }

        [Authorize(Roles = Constants.AdminRole)] // Μόνο ο διαχειριστής έχει την εξουσιοδώτηση για αυτή τη συνάρτηση
        public IActionResult AddEdit(int? id)
        {
            AddEdit Addedit = new AddEdit()
            {
                Part = new Part(),
                TypeList = _context.Types.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.ID.ToString()
                })
            };
            if (id == null) //εάν δεν υπάρχει ταυτότητα
            { 
                return View(Addedit);//Δημιουργία
            }
            else 
            { 
                Addedit.Part = _context.Parts.Find(id);
                if (Addedit.Part == null) //εάν δεν υπάρχει το εξάρτημα στη βάση
                { 
                    return NotFound();
                }
                return View(Addedit);//Επεξεργασία
            }
        }

        [Authorize(Roles = Constants.AdminRole)]// Μόνο ο διαχειριστής έχει την εξουσιοδώτηση για αυτή τη συνάρτηση
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddEdit(AddEdit Addedit)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _hostEnvironment.WebRootPath;

                if (Addedit.Part.ID == 0) //Δημιουργία
                {
                    string upload = webRootPath + Constants.ImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);
                     
                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    Addedit.Part.Image = fileName + extension;
                    _context.Parts.Add(Addedit.Part);
                }
                else //Αλλαγή
                {
                    var objFromDb = _context.Parts.AsNoTracking().FirstOrDefault(u => u.ID == Addedit.Part.ID);
                    if (files.Count > 0)
                    {
                        string upload = webRootPath + Constants.ImagePath;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);
                        var oldFile = Path.Combine(upload, objFromDb.Image);
                        if (System.IO.File.Exists(oldFile)) 
                        { 
                            System.IO.File.Delete(oldFile);
                        }
                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream); // αντιγραφή στον φάκελο φωτογραφιών
                        }
                        Addedit.Part.Image = fileName + extension;
                    }
                    else
                    { 
                        Addedit.Part.Image = objFromDb.Image;
                    }
                    _context.Parts.Update(Addedit.Part);
                }
                _context.SaveChanges();
                return RedirectToAction("List");
            }
            Addedit.TypeList = _context.Types.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.ID.ToString()
            });
            return View(Addedit);
        }

        public async Task<ActionResult> Details(int id)
        {
            Details details = new Details()
            {
                Part = await _context.Parts.Include(u => u.Type).Where(u => u.ID == id).FirstOrDefaultAsync()
            };
            return View(details);
        }

        public IActionResult AddToCart(int id, string filter, int page)
        {
            ShoppingCartId = GetCartId();
            var cartItem = _context.ShoppingCartItems.SingleOrDefault(c => c.CartId == ShoppingCartId && c.PartId == id);
            if (cartItem == null)
            {
                // Δημιουργία καινούργιου προϊόντος καλαθιού εάν δεν υπάρχει αυτό το προϊόν ήδη.                 
                cartItem = new CartItem
                {
                    PartId = id,
                    CartId = ShoppingCartId,
                    Part = _context.Parts.SingleOrDefault(p => p.ID == id),
                    Quantity = 1,
                    DateCreated = DateTime.Now
                };
                _context.ShoppingCartItems.Add(cartItem);
            }
            else
            {              
                cartItem.Quantity++;//αύξηση μονάδας κατά 1
            }
            _context.SaveChanges();
            if (filter != null)
            {
                ViewBag.CurrentFilter = filter;
                return RedirectToAction(nameof(List), new { filter = ViewBag.CurrentFilter, page = page });
            }
            return RedirectToAction(nameof(List), new {  page = page });
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
    }
}