using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace eShop.Areas.Identity.Data
{
    public class eShopUser : IdentityUser
    {
        [Required]
        public string Firstname { get; set; }

        [Required]
        public string Lastname { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string ZIP { get; set; }
    }
}
