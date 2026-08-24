using Microsoft.AspNetCore.Mvc;

namespace day05_S.Aress.Admin.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
