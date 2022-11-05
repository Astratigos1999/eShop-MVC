using System.ComponentModel.DataAnnotations;

namespace eShop.Models.Database
{
    public class CartItem
    {
        [Key]
        public int ItemId { get; set; }

        public string CartId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public System.DateTime DateCreated { get; set; }

        [Required]
        public int PartId { get; set; }

        public virtual Part Part { get; set; }// φόρτωση από τη βάση όταν χρειαστεί
    }
}
