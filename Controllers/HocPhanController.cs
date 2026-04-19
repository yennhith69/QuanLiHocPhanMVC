using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Data.SqlClient;
using QuanLyHocPhanMVC.Models;
using System.Text;

namespace QuanLyHocPhanMVC.Controllers
{
    public class HocPhanController : AppController
    {
        string connectionString = @"Server=(localdb)\mssqllocaldb;Database=QuanLyHocPhan;Trusted_Connection=True;TrustServerCertificate=True;";

        public IActionResult Index(string search, int page = 1)
        {
            var loginResult = RequireLogin();
            if (loginResult != null)
            {
                return loginResult;
            }

            List<HocPhan> list = new List<HocPhan>();

            int pageSize = 10;
            int start = (page - 1) * pageSize;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query;

                if (string.IsNullOrEmpty(search))
                {
                    query = @"SELECT * FROM HocPhan
                              ORDER BY IDHocPhan
                              OFFSET @start ROWS
                              FETCH NEXT @size ROWS ONLY";
                }
                else
                {
                    query = @"SELECT * FROM HocPhan
                              WHERE TenHocPhan LIKE @search
                              OR MaHocPhan LIKE @search
                              ORDER BY IDHocPhan
                              OFFSET @start ROWS
                              FETCH NEXT @size ROWS ONLY";
                }

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@start", start);
                cmd.Parameters.AddWithValue("@size", pageSize);
                if (!string.IsNullOrEmpty(search))
                {
                    cmd.Parameters.AddWithValue("@search", "%" + search + "%");
                }

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    HocPhan hp = new HocPhan();

                    hp.IDHocPhan = (int)reader["IDHocPhan"];
                    hp.MaHocPhan = reader["MaHocPhan"].ToString();
                    hp.TenHocPhan = reader["TenHocPhan"].ToString();
                    hp.SoTinChi = (int)reader["SoTinChi"];
                    hp.MoTa = reader["MoTa"].ToString();

                    list.Add(hp);
                }
            }

            ViewBag.Page = page;

            int totalRows = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM HocPhan", conn);

                totalRows = (int)cmd.ExecuteScalar();
            }

            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRows / pageSize);

            return View(list);
        }

        public IActionResult Create()
        {
            var roleResult = RequireRole("Admin");
            if (roleResult != null)
            {
                return roleResult;
            }

            return View();
        }

        [HttpPost]
        public IActionResult Create(HocPhan hp)
        {
            var roleResult = RequireRole("Admin");
            if (roleResult != null)
            {
                return roleResult;
            }

            if (!ModelState.IsValid)
            {
                return View(hp);
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"INSERT INTO HocPhan
                                (MaHocPhan,TenHocPhan,SoTinChi,MoTa)
                                VALUES
                                (@ma,@ten,@stc,@mota)";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@ma", hp.MaHocPhan);
                cmd.Parameters.AddWithValue("@ten", hp.TenHocPhan);
                cmd.Parameters.AddWithValue("@stc", hp.SoTinChi);
                cmd.Parameters.AddWithValue("@mota", hp.MoTa);

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var roleResult = RequireRole("Admin");
            if (roleResult != null)
            {
                return roleResult;
            }

            HocPhan hp = new HocPhan();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT * FROM HocPhan WHERE IDHocPhan=@id";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@id", id);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    hp.IDHocPhan = (int)reader["IDHocPhan"];
                    hp.MaHocPhan = reader["MaHocPhan"].ToString();
                    hp.TenHocPhan = reader["TenHocPhan"].ToString();
                    hp.SoTinChi = (int)reader["SoTinChi"];
                    hp.MoTa = reader["MoTa"].ToString();
                }
            }

            return View(hp);
        }

        [HttpPost]
        public IActionResult Edit(HocPhan hp)
        {
            var roleResult = RequireRole("Admin");
            if (roleResult != null)
            {
                return roleResult;
            }

            if (!ModelState.IsValid)
            {
                return View(hp);
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"UPDATE HocPhan
                                SET MaHocPhan=@ma,
                                    TenHocPhan=@ten,
                                    SoTinChi=@stc,
                                    MoTa=@mota
                                WHERE IDHocPhan=@id";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@id", hp.IDHocPhan);
                cmd.Parameters.AddWithValue("@ma", hp.MaHocPhan);
                cmd.Parameters.AddWithValue("@ten", hp.TenHocPhan);
                cmd.Parameters.AddWithValue("@stc", hp.SoTinChi);
                cmd.Parameters.AddWithValue("@mota", hp.MoTa);

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

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

                string deleteAssignQuery = "DELETE FROM PhanCongHocPhan WHERE IDHocPhan=@id";
                SqlCommand deleteAssignCmd = new SqlCommand(deleteAssignQuery, conn);
                deleteAssignCmd.Parameters.AddWithValue("@id", id);
                deleteAssignCmd.ExecuteNonQuery();

                string query = "DELETE FROM HocPhan WHERE IDHocPhan=@id";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@id", id);

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
        public IActionResult DanhSach(string search, int page = 1)
        {
            List<HocPhan> list = new List<HocPhan>();

            int pageSize = 10;
            int start = (page - 1) * pageSize;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query;

                if (string.IsNullOrEmpty(search))
                {
                    query = @"SELECT * FROM HocPhan
                      ORDER BY IDHocPhan
                      OFFSET @start ROWS
                      FETCH NEXT @size ROWS ONLY";
                }
                else
                {
                    query = @"SELECT * FROM HocPhan
                      WHERE TenHocPhan LIKE @search
                      OR MaHocPhan LIKE @search
                      ORDER BY IDHocPhan
                      OFFSET @start ROWS
                      FETCH NEXT @size ROWS ONLY";
                }

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@start", start);
                cmd.Parameters.AddWithValue("@size", pageSize);

                if (!string.IsNullOrEmpty(search))
                {
                    cmd.Parameters.AddWithValue("@search", "%" + search + "%");
                }

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    HocPhan hp = new HocPhan();

                    hp.IDHocPhan = (int)reader["IDHocPhan"];
                    hp.MaHocPhan = reader["MaHocPhan"].ToString();
                    hp.TenHocPhan = reader["TenHocPhan"].ToString();
                    hp.SoTinChi = (int)reader["SoTinChi"];
                    hp.MoTa = reader["MoTa"].ToString();

                    list.Add(hp);
                }
            }

            int totalRows = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string countQuery = "SELECT COUNT(*) FROM HocPhan";

                SqlCommand cmd = new SqlCommand(countQuery, conn);

                totalRows = (int)cmd.ExecuteScalar();
            }

            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRows / pageSize);
            ViewBag.Page = page;

            return View(list);
        }

        public IActionResult ExportCsv(string search)
        {
            var loginResult = RequireLogin();
            if (loginResult != null)
            {
                return loginResult;
            }

            List<HocPhan> list = new List<HocPhan>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"SELECT * FROM HocPhan";
                if (!string.IsNullOrEmpty(search))
                {
                    query += @" WHERE TenHocPhan LIKE @search OR MaHocPhan LIKE @search";
                }

                query += @" ORDER BY IDHocPhan";

                SqlCommand cmd = new SqlCommand(query, conn);
                if (!string.IsNullOrEmpty(search))
                {
                    cmd.Parameters.AddWithValue("@search", "%" + search + "%");
                }

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new HocPhan
                    {
                        IDHocPhan = (int)reader["IDHocPhan"],
                        MaHocPhan = reader["MaHocPhan"].ToString() ?? string.Empty,
                        TenHocPhan = reader["TenHocPhan"].ToString() ?? string.Empty,
                        SoTinChi = (int)reader["SoTinChi"],
                        MoTa = reader["MoTa"].ToString() ?? string.Empty
                    });
                }
            }

            var csv = new StringBuilder();
            csv.AppendLine("ID,MaHocPhan,TenHocPhan,SoTinChi,MoTa");
            foreach (var hp in list)
            {
                csv.AppendLine($"{hp.IDHocPhan},\"{hp.MaHocPhan.Replace("\"", "\"\"")}\",\"{hp.TenHocPhan.Replace("\"", "\"\"")}\",{hp.SoTinChi},\"{hp.MoTa.Replace("\"", "\"\"")}\"");
            }

            return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", $"hocphan_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
        }

        public IActionResult MonHocCuaToi(string search)
        {
            var loginResult = RequireLogin();
            if (loginResult != null)
            {
                return loginResult;
            }

            string userRole = CurrentRole;
            if (userRole != "GiaoVien")
            {
                ViewBag.ErrorMessage = "Chỉ giáo viên mới có quyền truy cập trang này!";
                return View("AccessDenied");
            }

            string username = HttpContext.Session.GetString("Username") ?? string.Empty;
            List<HocPhan> list = new List<HocPhan>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"SELECT hp.*
                                 FROM HocPhan hp
                                 INNER JOIN PhanCongHocPhan pc ON hp.IDHocPhan = pc.IDHocPhan
                                 INNER JOIN NguoiDung nd ON pc.IDNguoiDung = nd.ID
                                 WHERE nd.Username = @username";

                if (!string.IsNullOrEmpty(search))
                {
                    query += " AND (hp.TenHocPhan LIKE @search OR hp.MaHocPhan LIKE @search)";
                }

                query += " ORDER BY hp.IDHocPhan";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", username);

                if (!string.IsNullOrEmpty(search))
                {
                    cmd.Parameters.AddWithValue("@search", "%" + search + "%");
                }

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    HocPhan hp = new HocPhan();

                    hp.IDHocPhan = (int)reader["IDHocPhan"];
                    hp.MaHocPhan = reader["MaHocPhan"].ToString();
                    hp.TenHocPhan = reader["TenHocPhan"].ToString();
                    hp.SoTinChi = (int)reader["SoTinChi"];
                    hp.MoTa = reader["MoTa"].ToString();

                    list.Add(hp);
                }
            }

            ViewBag.Search = search;
            return View(list);
        }

        public IActionResult SuaMoTaMonHocCuaToi(int id)
        {
            var loginResult = RequireLogin();
            if (loginResult != null)
            {
                return loginResult;
            }

            string userRole = CurrentRole;
            if (userRole != "GiaoVien")
            {
                ViewBag.ErrorMessage = "Chỉ giáo viên mới có quyền truy cập trang này!";
                return View("AccessDenied");
            }

            string username = HttpContext.Session.GetString("Username") ?? string.Empty;

            if (!KiemTraHocPhanPhuTrach(id, username))
            {
                ViewBag.ErrorMessage = "Bạn không phụ trách học phần này!";
                return View("AccessDenied");
            }

            HocPhan hp = LayHocPhanTheoId(id);
            if (hp == null)
            {
                return NotFound();
            }

            return View(hp);
        }

        [HttpPost]
        public IActionResult SuaMoTaMonHocCuaToi(HocPhan hp)
        {
            var loginResult = RequireLogin();
            if (loginResult != null)
            {
                return loginResult;
            }

            string userRole = CurrentRole;
            if (userRole != "GiaoVien")
            {
                ViewBag.ErrorMessage = "Chỉ giáo viên mới có quyền truy cập trang này!";
                return View("AccessDenied");
            }

            if (!ModelState.IsValid)
            {
                return View(hp);
            }

            string username = HttpContext.Session.GetString("Username") ?? string.Empty;

            if (!KiemTraHocPhanPhuTrach(hp.IDHocPhan, username))
            {
                ViewBag.ErrorMessage = "Bạn không phụ trách học phần này!";
                return View("AccessDenied");
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"UPDATE HocPhan
                                 SET MoTa=@mota
                                 WHERE IDHocPhan=@id";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@id", hp.IDHocPhan);
                cmd.Parameters.AddWithValue("@mota", hp.MoTa ?? string.Empty);

                cmd.ExecuteNonQuery();
            }

            TempData["SuccessMessage"] = "Cập nhật mô tả học phần thành công.";
            return RedirectToAction("MonHocCuaToi");
        }

        public IActionResult ChiTiet(int id)
        {
            HocPhan hp = new HocPhan();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT * FROM HocPhan WHERE IDHocPhan=@id";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@id", id);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    hp.IDHocPhan = (int)reader["IDHocPhan"];
                    hp.MaHocPhan = reader["MaHocPhan"].ToString();
                    hp.TenHocPhan = reader["TenHocPhan"].ToString();
                    hp.SoTinChi = (int)reader["SoTinChi"];
                    hp.MoTa = reader["MoTa"].ToString();
                }
            }

            return View(hp);
        }

        private bool KiemTraHocPhanPhuTrach(int idHocPhan, string username)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"SELECT COUNT(*)
                                 FROM PhanCongHocPhan pc
                                 INNER JOIN NguoiDung nd ON pc.IDNguoiDung = nd.ID
                                 WHERE pc.IDHocPhan = @idHocPhan AND nd.Username = @username";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idHocPhan", idHocPhan);
                cmd.Parameters.AddWithValue("@username", username);

                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        private HocPhan? LayHocPhanTheoId(int id)
        {
            HocPhan hp = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT * FROM HocPhan WHERE IDHocPhan=@id";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@id", id);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    hp = new HocPhan
                    {
                        IDHocPhan = (int)reader["IDHocPhan"],
                        MaHocPhan = reader["MaHocPhan"].ToString(),
                        TenHocPhan = reader["TenHocPhan"].ToString(),
                        SoTinChi = (int)reader["SoTinChi"],
                        MoTa = reader["MoTa"].ToString()
                    };
                }
            }

            return hp;
        }
    }
}