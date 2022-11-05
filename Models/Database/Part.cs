using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eShop.Models.Database
{
    public class Part
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "Το όνομα είναι απαραίτητο.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Η περιγραφή είναι απαραίτητη.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Η τιμή είναι απαραίτητη.")]
        [Range(0,int.MaxValue)]
        public int Price { get; set; }

        public string Image { get; set; }

        [Required(ErrorMessage = "Η κατηγορία οχήματος είναι απαραίτητη.")]
        public string VehicleCategory { get; set; }

        [Required(ErrorMessage = "Ο τύπος εξαρτήματος είναι απαραίτητος.")]
        public int TypeID { get; set; }
        [ForeignKey("TypeID")]
        public virtual Type Type { get; set; } // φόρτωση από τη βάση όταν χρειαστεί
    }
}
