using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PointOfSale.Model
{
    public partial class TypeDocumentSale
    {
        public TypeDocumentSale()
        {
            Sales = new HashSet<Sale>();
        }
        [Key]

        public int IdTypeDocumentSale { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? RegistrationDate { get; set; }

        public virtual ICollection<Sale> Sales { get; set; }
    }
}
