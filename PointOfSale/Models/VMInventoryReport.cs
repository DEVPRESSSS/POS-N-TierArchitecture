namespace PointOfSale.Models
{
	public class VMInventoryReport
	{
		public string ProductName { get; set; }
		public int InitialStock { get; set; }
		public int QuantitySold { get; set; }
		public int RemainingStock { get; set; }
	}
}
