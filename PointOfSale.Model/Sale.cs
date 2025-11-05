using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PointOfSale.Model
{
    public partial class Sale
    {
        public Sale()
        {
            DetailSales = new HashSet<DetailSale>();
        }
        [Key]

        public int IdSale { get; set; }
        public string? SaleNumber { get; set; }
        public int? IdTypeDocumentSale { get; set; }

        [Required]
        public string? IdUsers { get; set; }
        [ForeignKey(nameof(IdUsers))]
        public ApplicationUser User { get; set; }
        public string? CustomerDocument { get; set; }
        public string? ClientName { get; set; }
        public decimal? Subtotal { get; set; }
        public decimal? TotalTaxes { get; set; }
        public decimal? Total { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string? PaymentType { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? AmountPaid { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? ChangeAmount { get; set; }
        public virtual TypeDocumentSale? IdTypeDocumentSaleNavigation { get; set; }
       // public virtual User? IdUsersNavigation { get; set; }
        public virtual ICollection<DetailSale> DetailSales { get; set; }
    }
}
