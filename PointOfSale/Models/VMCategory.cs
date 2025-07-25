using PointOfSale.Utilities.Regex;
using System.ComponentModel.DataAnnotations;

namespace PointOfSale.Models
{
    public class VMCategory
    {
        public int IdCategory { get; set; }

        public string? Description { get; set; }
        public int? IsActive { get; set; }
    }
}
