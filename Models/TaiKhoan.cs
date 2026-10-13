// Họ và tên: Nguyễn Minh Đức
// Mã sinh viên: 23203100078
// Nội dung thực hiện: Module 1 - Bổ sung Display và thông báo lỗi tiếng Việt cho Entity TaiKhoan
using System.ComponentModel.DataAnnotations;

namespace QuanLyDeTaiDoAn.Models
{
    public class TaiKhoan
    {
        [Key]
        public int MaTaiKhoan { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [StringLength(50, ErrorMessage = "Tên đăng nhập tối đa 50 ký tự")]
        [Display(Name = "Tên đăng nhập")]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(100, ErrorMessage = "Mật khẩu tối đa 100 ký tự")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string MatKhau { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100, ErrorMessage = "Email tối đa 100 ký tự")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        // "Admin" hoặc "SinhVien"
        [Required(ErrorMessage = "Vui lòng chọn vai trò")]
        [StringLength(20, ErrorMessage = "Vai trò tối đa 20 ký tự")]
        [Display(Name = "Vai trò")]
        public string VaiTro { get; set; } = string.Empty;

        // true = đang hoạt động, false = bị khóa
        [Display(Name = "Đang hoạt động")]
        public bool TrangThai { get; set; } = true;

        public SinhVien? SinhVien { get; set; }
    }
}
