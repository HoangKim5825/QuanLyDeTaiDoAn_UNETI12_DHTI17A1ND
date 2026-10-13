// Họ và tên: <ĐIỀN TÊN>
// Mã sinh viên: <ĐIỀN MÃ SV>
// Nội dung thực hiện: Module 3 - ViewModel quản lý sinh viên (Admin) và hồ sơ cá nhân (Sinh viên)

using System.ComponentModel.DataAnnotations;
using QuanLyDeTaiDoAn.Models;

namespace QuanLyDeTaiDoAn.ViewModels
{
    public class SinhVienListItem
    {
        public int MaSinhVien { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string? Lop { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? SoDienThoai { get; set; }
        public string TenDangNhap { get; set; } = string.Empty;
        public int SoDangKy { get; set; }
        public bool TrangThai { get; set; }
    }

    public class SinhVienFilterViewModel
    {
        public string? Keyword { get; set; }
        public bool? TrangThai { get; set; }      // true = hoạt động, false = ngừng
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public PagedResult<SinhVienListItem> KetQua { get; set; } = new();

        // Dùng cho asp-all-route-data ở link phân trang để giữ điều kiện tìm/lọc
        public Dictionary<string, string?> RouteValues(int page) => new()
        {
            ["Keyword"] = Keyword,
            ["TrangThai"] = TrangThai?.ToString().ToLower(),
            ["PageSize"] = PageSize.ToString(),
            ["Page"] = page.ToString()
        };
    }

    // Form Admin thêm/sửa sinh viên
    public class SinhVienFormViewModel
    {
        public int MaSinhVien { get; set; }

        // Chỉ chọn khi thêm mới; kiểm tra bắt buộc ở Controller
        [Display(Name = "Tài khoản đăng nhập")]
        public int? MaTaiKhoan { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập ngày sinh")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime? NgaySinh { get; set; }

        [StringLength(20, ErrorMessage = "Lớp tối đa 20 ký tự")]
        [Display(Name = "Lớp")]
        public string? Lop { get; set; }

        [StringLength(100, ErrorMessage = "Khoa tối đa 100 ký tự")]
        [Display(Name = "Khoa")]
        public string? Khoa { get; set; }

        [StringLength(100, ErrorMessage = "Chuyên ngành tối đa 100 ký tự")]
        [Display(Name = "Chuyên ngành")]
        public string? ChuyenNganh { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100, ErrorMessage = "Email tối đa 100 ký tự")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [RegularExpression(@"^(0|\+84)\d{9,10}$", ErrorMessage = "Số điện thoại không hợp lệ (VD: 0912345678)")]
        [StringLength(15, ErrorMessage = "Số điện thoại tối đa 15 ký tự")]
        [Display(Name = "Số điện thoại")]
        public string? SoDienThoai { get; set; }

        [Display(Name = "Đang hoạt động")]
        public bool TrangThai { get; set; } = true;
    }

    public class SinhVienDetailsViewModel
    {
        public SinhVien SinhVien { get; set; } = new();
        public List<DangKyCuaToiItem> DangKys { get; set; } = new();
    }

    // Form Sinh viên tự cập nhật: CHỈ cho sửa Email và Số điện thoại
    public class HoSoCapNhatViewModel
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100, ErrorMessage = "Email tối đa 100 ký tự")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [RegularExpression(@"^(0|\+84)\d{9,10}$", ErrorMessage = "Số điện thoại không hợp lệ (VD: 0912345678)")]
        [StringLength(15, ErrorMessage = "Số điện thoại tối đa 15 ký tự")]
        [Display(Name = "Số điện thoại")]
        public string? SoDienThoai { get; set; }

        // ===== Chỉ để hiển thị (không cho sửa) =====
        public int MaSinhVien { get; set; }
        public string? HoTen { get; set; }
        public DateTime NgaySinh { get; set; }
        public string? Lop { get; set; }
        public string? Khoa { get; set; }
        public string? ChuyenNganh { get; set; }
    }
}
