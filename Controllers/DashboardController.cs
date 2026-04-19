using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Text;

namespace QuanLyHocPhanMVC.Controllers
{
    public class DashboardController : AppController
    {
        private readonly string connectionString = @"Server=(localdb)\mssqllocaldb;Database=QuanLyHocPhan;Trusted_Connection=True;TrustServerCertificate=True;";

        public IActionResult Index()
        {
            var loginResult = RequireLogin();
            if (loginResult != null)
            {
                return loginResult;
            }

            string userRole = CurrentRole;
            string username = HttpContext.Session.GetString("Username") ?? string.Empty;

            ViewBag.Username = username;
            ViewBag.Role = userRole;

            if (string.Equals(userRole, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                LoadAdminDashboard();
            }
            else if (string.Equals(userRole, "GiaoVien", StringComparison.OrdinalIgnoreCase))
            {
                LoadTeacherDashboard();
            }

            return View();
        }

        public IActionResult ExportCsv()
        {
            var loginResult = RequireLogin();
            if (loginResult != null)
            {
                return loginResult;
            }

            if (!string.Equals(CurrentRole, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.ErrorMessage = "Bạn không có quyền truy cập!";
                return View("AccessDenied");
            }

            int tongHocPhan = 0;
            int tongTinChi = 0;
            int tongNguoiDung = 0;
            int soAdmin = 0;
            int soGiaoVien = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                tongHocPhan = Convert.ToInt32(new SqlCommand("SELECT COUNT(*) FROM HocPhan", conn).ExecuteScalar());
                var tongTinChiResult = new SqlCommand("SELECT SUM(SoTinChi) FROM HocPhan", conn).ExecuteScalar();
                tongTinChi = tongTinChiResult != null ? Convert.ToInt32(tongTinChiResult) : 0;
                tongNguoiDung = Convert.ToInt32(new SqlCommand("SELECT COUNT(*) FROM NguoiDung", conn).ExecuteScalar());
                soAdmin = Convert.ToInt32(new SqlCommand("SELECT COUNT(*) FROM NguoiDung WHERE Role='Admin'", conn).ExecuteScalar());
                soGiaoVien = Convert.ToInt32(new SqlCommand("SELECT COUNT(*) FROM NguoiDung WHERE Role='GiaoVien'", conn).ExecuteScalar());
            }

            var csv = new StringBuilder();
            csv.AppendLine("Chi tieu,Gia tri");
            csv.AppendLine($"Tong hoc phan,{tongHocPhan}");
            csv.AppendLine($"Tong tin chi,{tongTinChi}");
            csv.AppendLine($"Tong nguoi dung,{tongNguoiDung}");
            csv.AppendLine($"So Admin,{soAdmin}");
            csv.AppendLine($"So GiaoVien,{soGiaoVien}");

            return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", $"dashboard_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
        }

        private void LoadAdminDashboard()
        {
            int tongHocPhan = 0;
            int tongTinChi = 0;
            int tongNguoiDung = 0;
            int soAdmin = 0;
            int soGiaoVien = 0;

            List<int> tinChi = new List<int>();
            List<int> soLuong = new List<int>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Tổng học phần
                SqlCommand cmd1 = new SqlCommand("SELECT COUNT(*) FROM HocPhan", conn);
                tongHocPhan = (int)cmd1.ExecuteScalar();

                // Tổng tín chỉ
                SqlCommand cmd2 = new SqlCommand("SELECT SUM(SoTinChi) FROM HocPhan", conn);
                var result = cmd2.ExecuteScalar();
                tongTinChi = result != null ? (int)result : 0;

                // Tổng người dùng
                SqlCommand cmd3 = new SqlCommand("SELECT COUNT(*) FROM NguoiDung", conn);
                tongNguoiDung = (int)cmd3.ExecuteScalar();

                // Số Admin
                SqlCommand cmd4 = new SqlCommand("SELECT COUNT(*) FROM NguoiDung WHERE Role='Admin'", conn);
                soAdmin = (int)cmd4.ExecuteScalar();

                // Số Giáo viên
                SqlCommand cmd5 = new SqlCommand("SELECT COUNT(*) FROM NguoiDung WHERE Role='GiaoVien'", conn);
                soGiaoVien = (int)cmd5.ExecuteScalar();

                // Thống kê số môn theo tín chỉ
                SqlCommand cmd6 = new SqlCommand(@"
                SELECT SoTinChi, COUNT(*) as SoLuong
                FROM HocPhan
                GROUP BY SoTinChi
                ORDER BY SoTinChi
                ", conn);

                SqlDataReader reader = cmd6.ExecuteReader();

                while (reader.Read())
                {
                    tinChi.Add(Convert.ToInt32(reader["SoTinChi"]));
                    soLuong.Add(Convert.ToInt32(reader["SoLuong"]));
                }
            }

            ViewBag.TongHocPhan = tongHocPhan;
            ViewBag.TongTinChi = tongTinChi;
            ViewBag.TongNguoiDung = tongNguoiDung;
            ViewBag.SoAdmin = soAdmin;
            ViewBag.SoGiaoVien = soGiaoVien;

            ViewBag.TinChi = tinChi;
            ViewBag.SoLuong = soLuong;

            ViewBag.IsAdmin = true;
        }

        private void LoadTeacherDashboard()
        {
            int tongHocPhan = 0;
            int tongTinChi = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Tổng học phần
                SqlCommand cmd1 = new SqlCommand("SELECT COUNT(*) FROM HocPhan", conn);
                tongHocPhan = (int)cmd1.ExecuteScalar();

                // Tổng tín chỉ
                SqlCommand cmd2 = new SqlCommand("SELECT SUM(SoTinChi) FROM HocPhan", conn);
                var result = cmd2.ExecuteScalar();
                tongTinChi = result != null ? (int)result : 0;
            }

            ViewBag.TongHocPhan = tongHocPhan;
            ViewBag.TongTinChi = tongTinChi;
            ViewBag.IsAdmin = false;
        }
    }
}