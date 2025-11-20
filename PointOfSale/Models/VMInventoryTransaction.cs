namespace PointOfSale.Models
{
    public class VMInventoryTransaction
    {
        public string ProductName { get; set; }
        public string SaleNumber { get; set; }
        public DateTime SaleDate { get; set; }
        public int QuantitySold { get; set; }
        public int InitialStock { get; set; }
        public int RemainingStock { get; set; }
    }
}
