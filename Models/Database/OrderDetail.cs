using System.ComponentModel.DataAnnotations;

namespace eShop.Models.Database
{
    public class OrderDetail
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int OrderID { get; set; }

        public string Customer { get; set; }

        [Required]
        public int PartID { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public int UnitPrice { get; set; }

        public virtual Part Part { get; set; } // φόρτωση από τη βάση όταν χρειαστεί
    }
}
