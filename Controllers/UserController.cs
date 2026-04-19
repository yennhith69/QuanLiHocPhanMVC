using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Data.SqlClient;
using QuanLyHocPhanMVC.Models;

namespace QuanLyHocPhanMVC.Controllers
{
    public class UserController : AppController
    {
        string connectionString = @"Server=(localdb)\mssqllocaldb;Database=QuanLyHocPhan;Trusted_Connection=True;TrustServerCertificate=True;";

        public IActionResult Index(string search, int page = 1)
        {
            var roleResult = RequireRole("Admin");
            if (roleResult != null)
            {
                return roleResult;
            }

            List<NguoiDung> list = new List<NguoiDung>();

            int pageSize = 10;
            int start = (page - 1) * pageSize;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query;

                if (string.IsNullOrEmpty(search))
                {
                    query = @"SELECT * FROM NguoiDung
                              ORDER BY ID
                              OFFSET @start ROWS
                              FETCH NEXT @size ROWS ONLY";
                }
                else
                {
                    query = @"SELECT * FROM NguoiDung
                              WHERE Username LIKE @search
                              OR Role LIKE @search
                              ORDER BY ID
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
                    NguoiDung user = new NguoiDung();

                    user.ID = (int)reader["ID"];
                    user.Username = reader["Username"].ToString();
                    user.Password = reader["Password"].ToString();
                    user.Role = reader["Role"].ToString();

                    list.Add(user);
                }
            }

            ViewBag.Page = page;

            int totalRows = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM NguoiDung", conn);

                totalRows = (int)cmd.ExecuteScalar();
            }

            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRows / pageSize);
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

            return View();
        }

        [HttpPost]
        public IActionResult Create(NguoiDung user)
        {
            var roleResult = RequireRole("Admin");
            if (roleResult != null)
            {
                return roleResult;
            }

            if (!ModelState.IsValid)
            {
                return View(user);
            }

            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            {
                ViewBag.Error = "Username và Password không được để trống!";
                return View();
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Kiểm tra username đã tồn tại
                string checkQuery = "SELECT COUNT(*) FROM NguoiDung WHERE Username=@username";
                SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@username", user.Username);
                
                int count = (int)checkCmd.ExecuteScalar();
                if (count > 0)
                {
                    ViewBag.Error = "Username này đã tồn tại!";
                    return View();
                }

                string query = @"INSERT INTO NguoiDung
                                (Username, Password, Role)
                                VALUES
                                (@username, @password, @role)";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@username", user.Username);
                cmd.Parameters.AddWithValue("@password", user.Password);
                cmd.Parameters.AddWithValue("@role", user.Role ?? "GiaoVien");

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

            NguoiDung user = new NguoiDung();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT * FROM NguoiDung WHERE ID=@id";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@id", id);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    user.ID = (int)reader["ID"];
                    user.Username = reader["Username"].ToString();
                    user.Password = reader["Password"].ToString();
                    user.Role = reader["Role"].ToString();
                }
                else
                {
                    return NotFound();
                }
            }

            return View(user);
        }

        [HttpPost]
        public IActionResult Edit(NguoiDung user)
        {
            var roleResult = RequireRole("Admin");
            if (roleResult != null)
            {
                return roleResult;
            }

            if (!ModelState.IsValid)
            {
                return View(user);
            }

            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            {
                ViewBag.Error = "Username và Password không được để trống!";
                return View(user);
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"UPDATE NguoiDung
                                SET Username=@username,
                                    Password=@password,
                                    Role=@role
                                WHERE ID=@id";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@id", user.ID);
                cmd.Parameters.AddWithValue("@username", user.Username);
                cmd.Parameters.AddWithValue("@password", user.Password);
                cmd.Parameters.AddWithValue("@role", user.Role ?? "GiaoVien");

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

            NguoiDung user = new NguoiDung();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT * FROM NguoiDung WHERE ID=@id";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@id", id);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    user.ID = (int)reader["ID"];
                    user.Username = reader["Username"].ToString();
                    user.Password = reader["Password"].ToString();
                    user.Role = reader["Role"].ToString();
                }
                else
                {
                    return NotFound();
                }
            }

            return View(user);
        }

        [HttpPost]
        public IActionResult ConfirmDelete(int id)
        {
            var roleResult = RequireRole("Admin");
            if (roleResult != null)
            {
                return roleResult;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string deleteAssignQuery = "DELETE FROM PhanCongHocPhan WHERE IDNguoiDung=@id";
                SqlCommand deleteAssignCmd = new SqlCommand(deleteAssignQuery, conn);
                deleteAssignCmd.Parameters.AddWithValue("@id", id);
                deleteAssignCmd.ExecuteNonQuery();

                string query = "DELETE FROM NguoiDung WHERE ID=@id";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@id", id);

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
    }
}
