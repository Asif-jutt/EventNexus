using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventNexus.Views
{
    public class UserController : Controller
    {
        [Authorize(Roles = "User")]
        public IActionResult UserDashboard()
        {
            return View();
        }
    }
}
