using Microsoft.AspNetCore.Mvc;

namespace EventNexus.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult AdminDashboard()
        {
            return View();
        }
    }
}
