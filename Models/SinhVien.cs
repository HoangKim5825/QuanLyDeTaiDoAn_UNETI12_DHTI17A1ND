// Họ và tên: (Điền họ tên của bạn)
// Mã sinh viên: (Điền mã sinh viên của bạn)
// Nội dung thực hiện: Module 3 - Entity SinhVien (Quản lý sinh viên)

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyDeTaiDoAn.Models
{
    public class SinhVien
    {
        // MaSinhVien là mã số sinh viên thực tế (vd "22103100001"),
        // dùng luôn làm khóa chính -> không cần thêm cột Id phụ.
        [Key]
        [Required(ErrorMessage = "Mã sinh viên không được để trống")]
        [StringLength(20)]
        [Display(Name = "Mã sinh viên")]
        public string MaSinhVien { get; set; }

        [Required]
        [Display(Name = "Tài khoản")]
        public int MaTaiKhoan { get; set; }

        [ForeignKey(nameof(MaTaiKhoan))]
        public TaiKhoan TaiKhoan { get; set; }

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
        [StringLength(20)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } // "Đang học" / "Ngừng học"

        public ICollection<DangKyDeTai> DangKyDeTais { get; set; } = new List<DangKyDeTai>();
    }
}
