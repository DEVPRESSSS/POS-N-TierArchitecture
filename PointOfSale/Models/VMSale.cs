using PointOfSale.Model;

namespace PointOfSale.Models
{
    public class VMSale
    {
        public int IdSale { get; set; }
        public string? SaleNumber { get; set; }
        public int? IdTypeDocumentSale { get; set; }
        public string? TypeDocumentSale { get; set; }
        public string? IdUsers { get; set; }
        public string? Users { get; set; }
        public string? CustomerDocument { get; set; }
        public string? ClientName { get; set; }
        public string? Subtotal { get; set; }
        public string? TotalTaxes { get; set; }
        public string? Total { get; set; }

        //New
        public string? PaymentType { get; set; }
        public string? AmountPaid { get; set; }
        public string? Change { get; set; }

        //New
        public string? CashierName { get; set; }
        //
        public string? RegistrationDate { get; set; }
        public virtual ICollection<VMDetailSale> DetailSales { get; set; }
    }
}
