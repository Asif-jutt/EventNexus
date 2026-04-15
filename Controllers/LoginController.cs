using Microsoft.AspNetCore.Mvc;

namespace EventNexus.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
