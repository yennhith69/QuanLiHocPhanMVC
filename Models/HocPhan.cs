using System.ComponentModel.DataAnnotations;

namespace QuanLyHocPhanMVC.Models
{
    public class HocPhan
    {
        public int IDHocPhan { get; set; }

        [Required(ErrorMessage = "Mã học phần không được để trống")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Mã học phần phải từ 2 đến 20 ký tự")]
        public string MaHocPhan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên học phần không được để trống")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Tên học phần phải từ 3 đến 200 ký tự")]
        public string TenHocPhan { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Số tín chỉ phải lớn hơn 0")]
        public int SoTinChi { get; set; }

        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
        public string MoTa { get; set; } = string.Empty;
    }
}