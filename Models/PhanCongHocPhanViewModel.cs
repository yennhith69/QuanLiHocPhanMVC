namespace QuanLyHocPhanMVC.Models
{
    public class PhanCongHocPhanViewModel
    {
        public int ID { get; set; }

        public int IDNguoiDung { get; set; }

        public string Username { get; set; } = string.Empty;

        public int IDHocPhan { get; set; }

        public string MaHocPhan { get; set; } = string.Empty;

        public string TenHocPhan { get; set; } = string.Empty;

        public DateTime NgayPhanCong { get; set; }
    }
}
