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

            // Format the endDate correctly for your service
            string endDate = reportDate.Value.ToString("dd/MM/yyyy");
            string startDate = "01/01/2000"; // safe default start date

            var detailSales = await _saleService.Report(startDate, endDate);

            var inventoryReport = products.Select(p =>
            {
                var quantitySold = detailSales
                    .Where(d => d.IdProduct == p.IdProduct)
                    .Sum(d => d.Quantity ?? 0);

                return new VMInventoryReport
                {
                    ProductName = p.Description ?? p.Brand ?? "Unknown",
                    InitialStock = p.AllStock ?? 0,
                    QuantitySold = quantitySold,
                    RemainingStock = (p.AllStock ?? 0) - quantitySold
                };
            }).ToList();

            return StatusCode(StatusCodes.Status200OK, new { data = inventoryReport });
        }



    }
}
