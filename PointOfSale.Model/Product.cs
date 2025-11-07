using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PointOfSale.Model
{
    public partial class Product
    {
        [Key]
        public int IdProduct { get; set; }
        public string? BarCode { get; set; }
        public string? Brand { get; set; }
        public string? Description { get; set; }
        public int? IdCategory { get; set; }
        public int? Quantity { get; set; }
        public int? AllStock { get; set; }
        public decimal? Price { get; set; }
        public byte[]? Photo { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? RegistrationDate { get; set; }

        public virtual Category? IdCategoryNavigation { get; set; }
    }
}
