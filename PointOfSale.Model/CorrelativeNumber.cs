using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PointOfSale.Model
{
    public partial class CorrelativeNumber
    {
        [Key]
        public int IdCorrelativeNumber { get; set; }
        public int? LastNumber { get; set; }
        public int? QuantityDigits { get; set; }
        public string? Management { get; set; }
        public DateTime? DateUpdate { get; set; }
    }
}
