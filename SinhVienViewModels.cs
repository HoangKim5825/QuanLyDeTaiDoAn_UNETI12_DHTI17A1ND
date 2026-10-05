// Họ và tên: (Điền họ tên của bạn)
// Mã sinh viên: (Điền mã sinh viên của bạn)
// Nội dung thực hiện: Module 3 - ViewModel hỗ trợ quản lý sinh viên

using System.ComponentModel.DataAnnotations;

namespace QuanLyDeTaiDoAn.ViewModels
{
    /// <summary>
    /// Dùng cho form Admin tạo mới Sinh viên. Vì Sinh viên có quan hệ 1-1 bắt buộc
    /// với Tài khoản đăng nhập nên form Create gộp luôn 2 phần: thông tin sinh viên
    /// + tài khoản đăng nhập, tạo trong cùng 1 giao dịch (xem SinhVienController.Create).
    /// </summary>
    public class SinhVienCreateViewModel
    {
        [Required(ErrorMessage = "Mã sinh viên không được để trống")]
        [StringLength(20)]
        [Display(Name = "Mã sinh viên")]
        public string MaSinhVien { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100)]
        [Display(Name = "Họ tên")]
        public string HoTen { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime? NgaySinh { get; set; }

        [StringLength(20)]
        [Display(Name = "Lớp")]
        public string Lop { get; set; }

        [StringLength(100)]
        [Display(Name = "Khoa")]
        public string Khoa { get; set; }

        [StringLength(100)]
        [Display(Name = "Chuyên ngành")]
        public string ChuyenNganh { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100)]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(15)]
        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; }

        [Required]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "Đang học";

        // ----- Thông tin tài khoản đăng nhập đi kèm -----
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [StringLength(50)]
        [Display(Name = "Tên đăng nhập")]
        public string TenDangNhap { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(100)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string MatKhau { get; set; }
    }

    /// <summary>Form cho sinh viên tự cập nhật thông tin cá nhân (chỉ các trường được phép sửa).</summary>
    public class ThongTinCaNhanViewModel
    {
        public string MaSinhVien { get; set; }
        public string HoTen { get; set; }
        public string Lop { get; set; }
        public string Khoa { get; set; }
        public string ChuyenNganh { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100)]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(15)]
        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; }
    }
}
