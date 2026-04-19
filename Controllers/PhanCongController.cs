using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyHocPhanMVC.Models;
using System.Data.SqlClient;

namespace QuanLyHocPhanMVC.Controllers
{
    public class PhanCongController : AppController
    {
        private readonly string connectionString = @"Server=(localdb)\mssqllocaldb;Database=QuanLyHocPhan;Trusted_Connection=True;TrustServerCertificate=True;";

        public IActionResult Index(string search)
        {
            var roleResult = RequireRole("Admin");
            if (roleResult != null)
            {
                return roleResult;
            }

            List<PhanCongHocPhanViewModel> list = new List<PhanCongHocPhanViewModel>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"SELECT pc.ID, pc.IDNguoiDung, pc.IDHocPhan, pc.NgayPhanCong,
                                        nd.Username, hp.MaHocPhan, hp.TenHocPhan
                                 FROM PhanCongHocPhan pc
                                 INNER JOIN NguoiDung nd ON pc.IDNguoiDung = nd.ID
                                 INNER JOIN HocPhan hp ON pc.IDHocPhan = hp.IDHocPhan";

                if (!string.IsNullOrEmpty(search))
                {
                    query += " WHERE nd.Username LIKE @search OR hp.MaHocPhan LIKE @search OR hp.TenHocPhan LIKE @search";
                }

                query += " ORDER BY pc.NgayPhanCong DESC, nd.Username ASC";

                SqlCommand cmd = new SqlCommand(query, conn);

                if (!string.IsNullOrEmpty(search))
                {
                    cmd.Parameters.AddWithValue("@search", "%" + search + "%");
                }

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new PhanCongHocPhanViewModel
                    {
                        ID = (int)reader["ID"],
                        IDNguoiDung = (int)reader["IDNguoiDung"],
                        IDHocPhan = (int)reader["IDHocPhan"],
                        Username = reader["Username"].ToString() ?? string.Empty,
                        MaHocPhan = reader["MaHocPhan"].ToString() ?? string.Empty,
                        TenHocPhan = reader["TenHocPhan"].ToString() ?? string.Empty,
                        NgayPhanCong = Convert.ToDateTime(reader["NgayPhanCong"])
                    });
                }
            }

            ViewBag.Search = search;
            return View(list);
        }

        public IActionResult Create()
        {
            var roleResult = RequireRole("Admin");
            if (roleResult != null)
            {
                return roleResult;
            }

            LoadDropdownData();
            return View();
        }

        [HttpPost]
        public IActionResult Create(int idNguoiDung, int idHocPhan)
        {
            var roleResult = RequireRole("Admin");
            if (roleResult != null)
            {
                return roleResult;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string checkQuery = @"SELECT COUNT(*)
                                      FROM PhanCongHocPhan
                                      WHERE IDNguoiDung=@idNguoiDung AND IDHocPhan=@idHocPhan";

                SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@idNguoiDung", idNguoiDung);
                checkCmd.Parameters.AddWithValue("@idHocPhan", idHocPhan);

                int exists = (int)checkCmd.ExecuteScalar();
                if (exists > 0)
                {
                    TempData["ErrorMessage"] = "Phân công đã tồn tại.";
                    return RedirectToAction("Index");
                }

                string query = @"INSERT INTO PhanCongHocPhan (IDNguoiDung, IDHocPhan)
                                 VALUES (@idNguoiDung, @idHocPhan)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idNguoiDung", idNguoiDung);
                cmd.Parameters.AddWithValue("@idHocPhan", idHocPhan);
                cmd.ExecuteNonQuery();
            }

            TempData["SuccessMessage"] = "Phân công giảng dạy thành công.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var roleResult = RequireRole("Admin");
            if (roleResult != null)
            {
                return roleResult;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "DELETE FROM PhanCongHocPhan WHERE ID=@id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }

            TempData["SuccessMessage"] = "Đã xóa phân công giảng dạy.";
            return RedirectToAction("Index");
        }

        private void LoadDropdownData()
        {
            List<SelectListItem> giaoVienList = new List<SelectListItem>();
            List<SelectListItem> hocPhanList = new List<SelectListItem>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string giaoVienQuery = @"SELECT ID, Username
                                         FROM NguoiDung
                                         WHERE Role='GiaoVien'
                                         ORDER BY Username";

                SqlCommand giaoVienCmd = new SqlCommand(giaoVienQuery, conn);
                SqlDataReader giaoVienReader = giaoVienCmd.ExecuteReader();

                while (giaoVienReader.Read())
                {
                    giaoVienList.Add(new SelectListItem
                    {
                        Value = giaoVienReader["ID"].ToString(),
                        Text = giaoVienReader["Username"].ToString()
                    });
                }

                giaoVienReader.Close();

                string hocPhanQuery = @"SELECT IDHocPhan, MaHocPhan, TenHocPhan
                                        FROM HocPhan
                                        ORDER BY TenHocPhan";

                SqlCommand hocPhanCmd = new SqlCommand(hocPhanQuery, conn);
                SqlDataReader hocPhanReader = hocPhanCmd.ExecuteReader();

                while (hocPhanReader.Read())
                {
                    hocPhanList.Add(new SelectListItem
                    {
                        Value = hocPhanReader["IDHocPhan"].ToString(),
                        Text = $"{hocPhanReader["MaHocPhan"]} - {hocPhanReader["TenHocPhan"]}"
                    });
                }
            }

            ViewBag.GiaoVienList = giaoVienList;
            ViewBag.HocPhanList = hocPhanList;
        }
    }
}
