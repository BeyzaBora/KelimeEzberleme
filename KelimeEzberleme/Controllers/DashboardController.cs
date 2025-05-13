using Microsoft.AspNetCore.Mvc;

namespace KelimeEzberleme.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View();
        }
    }
}
