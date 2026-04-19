using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using QuanLyHocPhanMVC.Models;

namespace QuanLyHocPhanMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly string connectionString;

        public AccountController(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("conn");
        }

        // Hiển thị trang Login
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        // Xử lý Login
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT Role FROM NguoiDung WHERE Username=@u AND Password=@p";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@u", model.Username);
                cmd.Parameters.AddWithValue("@p", model.Password);

                var result = cmd.ExecuteScalar();

                if (result != null)
                {
                    string role = result.ToString();
                    HttpContext.Session.SetString("Username", model.Username);
                    HttpContext.Session.SetString("Role", role);

                    return RedirectToAction("Index", "HocPhan");
                }
                else
                {
                    ViewBag.Error = "Sai username hoặc password!";
                    return View();
                }
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}