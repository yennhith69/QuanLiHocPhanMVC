using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace QuanLyHocPhanMVC.Controllers
{
    public abstract class AppController : Controller
    {
        protected bool IsAuthenticated() => !string.IsNullOrEmpty(HttpContext.Session.GetString("Username"));

        protected string CurrentRole => HttpContext.Session.GetString("Role") ?? string.Empty;

        protected IActionResult? RequireLogin()
        {
            if (!IsAuthenticated())
            {
                return RedirectToAction("Login", "Account");
            }

            return null;
        }

        protected IActionResult? RequireRole(string requiredRole)
        {
            var loginResult = RequireLogin();
            if (loginResult != null)
            {
                return loginResult;
            }

            if (!string.Equals(CurrentRole, requiredRole, StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.ErrorMessage = "Bạn không có quyền truy cập!";
                return View("AccessDenied");
            }

            return null;
        }

        protected IActionResult? RequireAnyRole(params string[] roles)
        {
            var loginResult = RequireLogin();
            if (loginResult != null)
            {
                return loginResult;
            }

            if (roles == null || roles.Length == 0)
            {
                return null;
            }

            if (!roles.Any(role => string.Equals(CurrentRole, role, StringComparison.OrdinalIgnoreCase)))
            {
                ViewBag.ErrorMessage = "Bạn không có quyền truy cập!";
                return View("AccessDenied");
            }

            return null;
        }
    }
}
