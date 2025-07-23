using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PointOfSale.Model
{
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }
            
        [Key]
        public int IdCategory { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? RegistrationDate { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
