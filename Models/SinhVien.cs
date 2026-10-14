using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyDeTaiDoAn.Models
{
    public class SinhVien
    {
        [Key]
        [Display(Name = "Mã sinh viên")]
        public int MaSinhVien { get; set; }

        [Display(Name = "Tài khoản")]
        public int MaTaiKhoan { get; set; }
        [ForeignKey("MaTaiKhoan")]
        public TaiKhoan? TaiKhoan { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime NgaySinh { get; set; }

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

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(15, ErrorMessage = "Số điện thoại tối đa 15 ký tự")]
        [Display(Name = "Số điện thoại")]
        public string? SoDienThoai { get; set; }

        [Display(Name = "Đang hoạt động")]
        public bool TrangThai { get; set; } = true;

        public ICollection<DangKyDeTai> DanhSachDangKy { get; set; } = new List<DangKyDeTai>();

    }
}