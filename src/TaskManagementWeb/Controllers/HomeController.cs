using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TaskManagementWeb.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                return Redirect("/Admin/Dashboard");
            }
            if (User.IsInRole("Manager"))
            {
                return Redirect("/Manager/Dashboard");
            }
            if (User.IsInRole("Employee"))
            {
                return Redirect("/Employee/Dashboard");
            }
            return View();
        }
    }
}