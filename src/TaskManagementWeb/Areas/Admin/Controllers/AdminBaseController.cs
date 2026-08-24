using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TaskManagementWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public abstract class AdminBaseController : Controller
    {
        protected int CurrentUserId
        {
            get
            {
                var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return int.TryParse(idClaim, out int userId) ? userId : 0;
            }
        }

        protected string CurrentUserName
        {
            get
            {
                return User.FindFirst(ClaimTypes.Name)?.Value ?? "Admin";
            }
        }
    }
}
