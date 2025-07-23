using Microsoft.AspNetCore.Mvc;

namespace PointOfSale.Areas.Cashier.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
