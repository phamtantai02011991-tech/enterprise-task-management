using Microsoft.AspNetCore.Mvc;

namespace TaskManagementWeb.Controllers
{
    public class BaseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
