using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointOfSale.Model
{
    public class ApplicationUser:IdentityUser
    {

        [Required]
        public string? Name { get; set; }
   
        [Required]
        public string? ProfilePath { get; set; }
        public DateTime? DateCreated { get; set; }

        public virtual ICollection<Sale> Sales { get; set; }

    }
}
