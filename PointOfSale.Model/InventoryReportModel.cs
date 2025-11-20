using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointOfSale.Model
{
	public class InventoryReportModel
	{
		public string? ProductName { get; set; }
		public int InitialStock { get; set; }
		public int QuantitySold { get; set; }
		public int RemainingStock { get; set; }
	}
}
