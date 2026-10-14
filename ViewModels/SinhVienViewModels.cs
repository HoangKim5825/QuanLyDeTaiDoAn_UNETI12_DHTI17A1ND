// Họ và tên: <ĐIỀN TÊN>
// Mã sinh viên: <ĐIỀN MÃ SV>
// Nội dung thực hiện: Module 3 - ViewModel quản lý sinh viên (danh sách/lọc/phân trang, hồ sơ cá nhân)
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace QuanLyDeTaiDoAn.ViewModels
{
    public class SinhVienListItem
    {
        public int MaSinhVien { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string? Lop { get; set; }
        public string? Khoa { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? SoDienThoai { get; set; }
        public bool TrangThai { get; set; }
        public string TenDangNhap { get; set; } = string.Empty;
        public int SoDangKy { get; set; }
    }

    public class SinhVienFilterViewModel
    {
        public string? Keyword { get; set; }
        public string? Lop { get; set; }
        public bool? TrangThai { get; set; }
        public int Page { get; set; } = 1;

        public PagedResult<SinhVienListItem> KetQua { get; set; } = new();
        public List<string> DanhSachLop { get; set; } = new();
    }

    // Sinh viên chỉ được sửa Email và Số điện thoại của chính mình
    public class SinhVienHoSoViewModel
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100, ErrorMessage = "Email tối đa 100 ký tự")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(15, ErrorMessage = "Số điện thoại tối đa 15 ký tự")]
        [Display(Name = "Số điện thoại")]
        public string? SoDienThoai { get; set; }

        // Chỉ để hiển thị, nạp lại từ DB, không bind
        [BindNever, ValidateNever] public int MaSinhVien { get; set; }
        [BindNever, ValidateNever] public string TenDangNhap { get; set; } = string.Empty;
        [BindNever, ValidateNever] public string HoTen { get; set; } = string.Empty;
        [BindNever, ValidateNever] public DateTime NgaySinh { get; set; }
        [BindNever, ValidateNever] public string? Lop { get; set; }
        [BindNever, ValidateNever] public string? Khoa { get; set; }
        [BindNever, ValidateNever] public string? ChuyenNganh { get; set; }
    }
}
