using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PointOfSale.Utilities;

namespace PointOfSale.Areas.Cashier.Controllers
{

    [Area("Cashier")]
    [Authorize(Roles =SD.Cashier)]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
