using EventNexus.Models;
using EventNexus.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace EventNexus.Controllers
{
    public class LoginController : Controller
    {
        private readonly EventNexusContext _context;
        private readonly JwtService _jwtService;

        public LoginController(EventNexusContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            var email = HttpContext.Session.GetString("Email");
            var role = HttpContext.Session.GetString("Role");
            if(string.IsNullOrEmpty(email))
            {
                return View();
            }
            else if(role=="Manager")
            {
                return RedirectToAction("ManagerDashboard", "Manager");
            }
            else if(role=="Admin")
            {
                return RedirectToAction("AdminDashboard", "Admin");
            }
            else
            {
                return RedirectToAction("UserDashboard", "User");
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RegisterUser(Register model)
        {
            if (string.IsNullOrEmpty(model.Email) ||
                string.IsNullOrEmpty(model.Password) ||
                string.IsNullOrEmpty(model.Role))
            {
                ViewBag.Message = "Please enter Email, Password and Role";
                return View("Register");
            }

            try
            {
                var newUser = new Register
                {
                    Email = model.Email,
                    Password = model.Password,
                    Role = model.Role
                };

                _context.Registers.Add(newUser);
                _context.SaveChanges();

                HttpContext.Session.SetString("Email", model.Email);
                HttpContext.Session.SetString("Role", model.Role);

                string token = _jwtService.GenerateToken(model.Email, model.Role);
                Response.Cookies.Append("jwt", token);

                if (model.Role == "Manager")
                    return RedirectToAction("ManagerDashboard", "Manager");
                else if (model.Role == "Admin")
                    return RedirectToAction("AdminDashboard", "Admin");
                else
                    return RedirectToAction("UserDashboard", "User");
            }
            catch
            {
                ViewBag.Error = "Registration failed";
                return View("Register");
            }
        }

        [HttpPost]
        public IActionResult LoginUser(Register model)
        {
            var user = _context.Registers
                .FirstOrDefault(x => x.Email == model.Email
                                  && x.Password == model.Password
                                  && x.Role == model.Role);

            if (user != null)
            {
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetString("Role", user.Role);

                string token = _jwtService.GenerateToken(user.Email, user.Role);
                Response.Cookies.Append("jwt", token);

                if (user.Role == "Admin")
                    return RedirectToAction("AdminDashboard", "Admin");

                else if (user.Role == "Manager")
                    return RedirectToAction("ManagerDashboard", "Manager");

                else
                    return RedirectToAction("UserDashboard", "User");
            }

            ViewBag.Error = "Invalid Email or Password";
            return View("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("jwt");
            return RedirectToAction("Login");
        }
    }
}