using System.ComponentModel.DataAnnotations;

namespace eShop.Models.Database
{
    public class Type
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "Το όνομα είναι απαραίτητο.")]
        public string Name { get; set; }
    }
}
