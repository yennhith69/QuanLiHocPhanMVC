using System.ComponentModel.DataAnnotations;

namespace QuanLyHocPhanMVC.Models
{
    public class NguoiDung
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Username không được để trống")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username phải từ 3 đến 50 ký tự")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password không được để trống")]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "Password phải có ít nhất 4 ký tự")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role không được để trống")]
        public string Role { get; set; } = string.Empty;
    }
}