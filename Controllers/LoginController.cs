using EventNexus.Models;
using EventNexus.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
namespace EventNexus.Controllers
{
    public class LoginController : Controller
    {
        string connectionstring = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=EventNexus;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        private readonly JwtService _jwtService;
        public LoginController(JwtService jwtService)
        {
            _jwtService = jwtService;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RegisterUser(LoginViewModel model)
        {
            if (string.IsNullOrEmpty(model.Email) ||
                string.IsNullOrEmpty(model.Password) ||
                string.IsNullOrEmpty(model.Role))
            {
                ViewBag.Message = "Please enter Email, Password and Role";
                return View("Register");
            }

            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                con.Open();

                string query = "INSERT INTO Register (Email, Password, Role) VALUES (@Email, @Password, @Role)";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Email", model.Email);
                cmd.Parameters.AddWithValue("@Password", model.Password);
                cmd.Parameters.AddWithValue("@Role", model.Role);

                int result = cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    // ✅ Set Session AFTER success
                    HttpContext.Session.SetString("Email", model.Email);
                    HttpContext.Session.SetString("Role", model.Role);

                    // ✅ Generate JWT AFTER success
                    string token = _jwtService.GenerateToken(model.Email, model.Role);
                    Response.Cookies.Append("jwt", token);
                    if(model.Role=="Manager")
                    {
                        return RedirectToAction("ManagerDashboard", "Manager");
                    }
                    else if(model.Role=="Admin")
                    {
                        return RedirectToAction("AdminDashboard", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("UserDashboard", "User");
                    }

                }
                else
                   {
                    ViewBag.Message = "Registration failed";
                    return View("Register");
                }
            }
        }

        public IActionResult LoginUser(LoginViewModel model)
        {
            
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                string query = "SELECT * FROM Register WHERE Email = @Email AND Password = @Password AND Role = @Role";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Email", model.Email);
                cmd.Parameters.AddWithValue("@Password", model.Password);
                cmd.Parameters.AddWithValue("@Role", model.Role);

                con.Open();
                SqlDataReader r = cmd.ExecuteReader();

                if (r.HasRows)
                {
                    HttpContext.Session.SetString("Email", model.Email);
                    HttpContext.Session.SetString("Role", model.Role);
                    string token = _jwtService.GenerateToken(model.Email, model.Role);
                    if (model.Role == "Admin")
                        return RedirectToAction("AdminDashboard", "Admin");

                    else if (model.Role == "Manager")
                        return RedirectToAction("ManagerDashboard", "Manager");

                    else
                        return RedirectToAction("UserDashboard", "User");
                }
                else
                {
                    ViewBag.Error = "Invalid Email or Password";
                    return View("Login");
                }
            }
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("jwt");
            return View("Login");
        }
    }
}
