using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PointOfSale.Business.Contracts;
using PointOfSale.Models;
using PointOfSale.Utilities;

namespace PointOfSale.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = SD.Admin)]
    public class ReportsController : Controller
    {
        private readonly ISaleService _saleService;
        private readonly IMapper _mapper;
        public ReportsController(ISaleService saleService, IMapper mapper)
        {
            _saleService = saleService;
            _mapper = mapper;
        }
        public IActionResult SalesReport()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ReportSale(string startDate, string endDate)
        {
            List<VMSalesReport> vmList = _mapper.Map<List<VMSalesReport>>(await _saleService.Report(startDate, endDate));
            return StatusCode(StatusCodes.Status200OK, new { data = vmList });
        }
    }
}
