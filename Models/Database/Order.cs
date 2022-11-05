using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace eShop.Models.Database
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        
        [Required]
        public System.DateTime OrderDate { get; set; }

        public string Customer { get; set; }

        [Required]
        public bool HasBeenShipped { get; set; }

        [Required]
        public int Total { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string PostalCode { get; set; }

        [DisplayName]
        public string Phone { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public List<OrderDetail> OrderDetails { get; set; }
    }
}
