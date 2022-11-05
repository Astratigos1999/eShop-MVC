using System.ComponentModel.DataAnnotations;

namespace eShop.Models.ViewModels
{
    public class Credentials
    {
        [Required(ErrorMessage = "Το όνομά σας είναι απαραίτητο.")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "Το επίθετό σας είναι απαραίτητο.")]
        public string Lastname { get; set; }

        [Required(ErrorMessage = "Η πόλη είναι απαραίτητη.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Η διεύθυνση είναι απαραίτητη.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Ο Τ.Κ. είναι απαραίτητος.")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Μόνο αριθμοί.")]
        [StringLength(5, ErrorMessage = "Ο Τ.Κ. πρέπει να είναι 5ψήφιος.")]
        public string ZIP { get; set; }

        [Required(ErrorMessage = "Το ηλεκτρονικό ταχυδρομείο είναι απαραίτητο.")]
        [EmailAddress(ErrorMessage = "Λάθος δομή ηλεκτρονικού ταχυδρομείου")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Το τηλέφωνο είναι απαραίτητο.")]
        [RegularExpression("^[0-9]+$", ErrorMessage ="Μόνο αριθμοί.")]
        [StringLength(10, ErrorMessage ="Το νούμερο του τηλεφώνου πρέπει να είναι 10ψήφιο.")]
        public string Phone { get; set; }
    }
}
