using Microsoft.AspNetCore.Mvc;
using PointOfSale.Utilities;

namespace PointOfSale.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult CheckUserRole()
        {

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }


            if (User.IsInRole(SD.Admin))
            {
                return RedirectToAction("Dashboard", "Admin", new { area = "Admin" });
            }
            else if (User.IsInRole(SD.Cashier))
            {
                return RedirectToAction("NewSale", "Sales", new { area = "Cashier" });
            }


            return RedirectToAction("NewSale", "Sales", new { area = "Cashier" });

        }
    }
}
