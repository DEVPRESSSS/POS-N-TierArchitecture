using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PointOfSale.Business.Contracts;
using PointOfSale.Business.Services;
using PointOfSale.Models;
using PointOfSale.Utilities;

namespace PointOfSale.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = SD.Admin)]
    public class ReportsController : Controller
    {
        private readonly ISaleService _saleService;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        public ReportsController(ISaleService saleService,IProductService productService, IMapper mapper)
        {
            _saleService = saleService;
            _productService = productService;
            _mapper = mapper;
        }
        public IActionResult SalesReport()
        {
            return View();
        }
        public IActionResult InventoryReport()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ReportSale(string startDate, string endDate)
        {
            List<VMSalesReport> vmList = _mapper.Map<List<VMSalesReport>>(await _saleService.Report(startDate, endDate));
            return StatusCode(StatusCodes.Status200OK, new { data = vmList });
        }

        [HttpGet]
        public async Task<IActionResult> GetInventoryReport(DateTime? reportDate = null)
        {
            reportDate ??= DateTime.Now;

            var products = await _productService.List();
            string startDate = "01/01/2000";
            string endDate = reportDate.Value.ToString("dd/MM/yyyy");

            var detailSales = await _saleService.Report(startDate, endDate);

            var report = new List<VMInventoryTransaction>();

            foreach (var p in products)
            {
                int remainingStock = p.AllStock ?? 0;

                // Find all transactions for this product
                var transactions = detailSales
                    .Where(d => d.IdProduct == p.IdProduct)
                    .OrderBy(d => d.IdSaleNavigation.RegistrationDate);

                foreach (var t in transactions)
                {
                    int quantitySold = t.Quantity ?? 0;
                    remainingStock -= quantitySold;

                    report.Add(new VMInventoryTransaction
                    {
                        ProductName = p.Description ?? p.Brand ?? "Unknown",
                        SaleNumber = t.IdSaleNavigation?.SaleNumber ?? "-",
                        SaleDate = t.IdSaleNavigation?.RegistrationDate ?? DateTime.MinValue,
                        QuantitySold = quantitySold,
                        InitialStock = p.AllStock ?? 0,
                        RemainingStock = remainingStock
                    });
                }

                // Optional: If product has no sales, still show initial stock
                if (!transactions.Any())
                {
                    report.Add(new VMInventoryTransaction
                    {
                        ProductName = p.Description ?? p.Brand ?? "Unknown",
                        SaleNumber = "-",
                        SaleDate = DateTime.MinValue,
                        QuantitySold = 0,
                        InitialStock = p.AllStock ?? 0,
                        RemainingStock = p.AllStock ?? 0
                    });
                }
            }

            return StatusCode(StatusCodes.Status200OK, new { data = report });
        }




    }
}
