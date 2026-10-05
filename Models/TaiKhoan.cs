using System.ComponentModel.DataAnnotations;

namespace QuanLyDeTaiDoAn.Models
{
    // Ghi chú: Entity này thuộc phạm vi phụ trách chính của Module 1
    // (Tài khoản - Đăng nhập - Phân quyền). Module 3 chỉ tham chiếu qua
    // navigation property SinhVien.TaiKhoan / TaiKhoan.SinhVien để lấy
    // thông tin đăng nhập, KHÔNG chỉnh sửa nghiệp vụ phân quyền ở đây.
    public class TaiKhoan
    {
        [Key]
        public int MaTaiKhoan { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [StringLength(50)]
        [Display(Name = "Tên đăng nhập")]
        public string TenDangNhap { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(100)]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100)]
        [Display(Name = "Họ tên")]
        public string HoTen { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Vai trò")]
        public string VaiTro { get; set; } // "Admin" hoặc "SinhVien" - xem Common.AppConstants

        [Required]
        [StringLength(20)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } // "Hoạt động" / "Khóa"

        // Quan hệ 1-1 với SinhVien (chỉ áp dụng khi VaiTro = SinhVien)
        public SinhVien SinhVien { get; set; }
    }
}
