
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace RaceDay.Mvc.Controllers
{
    public class OrganiserController : Controller
    {
        public IActionResult Index()
        {
            string? token = HttpContext.Session.GetString("AccessToken");
            string? role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrWhiteSpace(token))
                return RedirectToAction("Login", "Account");

            if (!string.Equals(role, "Organiser",
                StringComparison.OrdinalIgnoreCase))
                return StatusCode(403);

            return View();
        }
    }
}
