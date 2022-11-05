using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using eShop.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace eShop.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<eShopUser> _signInManager;
        private readonly UserManager<eShopUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        public RegisterModel( UserManager<eShopUser> userManager, SignInManager<eShopUser> signInManager, ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public string ReturnUrl { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Το όνομά σας είναι απαραίτητο.")]
            [Display(Name = "Όνομα")]
            public string Firstname { get; set; }

            [Required(ErrorMessage = "Το επίθετό σας είναι απαραίτητο.")]
            [Display(Name = "Επώνυμο")]
            public string Lastname { get; set; }

            [Required(ErrorMessage = "Η πόλη είναι απαραίτητη.")]
            [Display(Name = "Πόλη")]
            public string City { get; set; }

            [Required(ErrorMessage = "Ο Τ.Κ. είναι απαραίτητος.")]
            [RegularExpression("^[0-9]+$", ErrorMessage = "Μόνο αριθμοί.")]
            [StringLength(5, ErrorMessage = "Ο Τ.Κ. πρέπει να είναι 5ψήφιος.")]
            [Display(Name = "Ταχυδρομικός Κώδικας")]
            public string ZIP { get; set; }

            [Required(ErrorMessage = "Η διεύθυνση είναι απαραίτητη.")]
            [Display(Name = "Διεύθυνση")]
            public string Address { get; set; }

            [Required(ErrorMessage = "Το τηλέφωνο είναι απαραίτητο.")]
            [RegularExpression("^[0-9]+$", ErrorMessage = "Μόνο αριθμοί.")]
            [StringLength(10, ErrorMessage = "Το νούμερο του τηλεφώνου πρέπει να είναι 10ψήφιο.")]
            [Display(Name = "Τηλέφωνο")]
            public string Phone { get; set; }

            [Required(ErrorMessage = "Το ηλεκτρονικό ταχυδρομείο είναι απαραίτητο.")]
            [EmailAddress(ErrorMessage = "Λάθος δομή ηλεκτρονικού ταχυδρομείου")]
            [Display(Name = "Ηλεκτρονικό Ταχυδρομείο")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "Ο {0} πρέπει να είναι τουλάχιστον {2} και το πολύ {1} χαρακτήρες σε μήκος.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Κωδικός Πρόσβασης")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Επιβεβαίωση Κωδικού Πρόσβασης")]
            [Compare("Password", ErrorMessage = "Ο κωδικός πρόσβασης και η επιβεβαίωσή του δεν ταιριάζουν.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new eShopUser 
                { Firstname = Input.Firstname, Lastname = Input.Lastname, City = Input.City, Address = Input.Address, PhoneNumber = Input.Phone, ZIP = Input.ZIP, UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    /*var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",  $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                    */
                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            // Κάτι πήγε στραβά, επανεμφάνιση φόρμας
            return Page();
        }
    }
}
