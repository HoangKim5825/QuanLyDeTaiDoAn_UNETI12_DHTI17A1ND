using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyDeTaiDoAn.Models
{
    public class SinhVien
    {
        [Key]
        public int MaSinhVien { get; set; }

        public int MaTaiKhoan { get; set; }
        [ForeignKey("MaTaiKhoan")]
        public TaiKhoan? TaiKhoan { get; set; }

        [Required, StringLength(100)]
        public string HoTen { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime NgaySinh { get; set; }

        [StringLength(20)]
        public string? Lop { get; set; }

        [StringLength(100)]
        public string? Khoa { get; set; }

        [StringLength(100)]
        public string? ChuyenNganh { get; set; }

        [Required, EmailAddress, StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Phone, StringLength(15)]
        public string? SoDienThoai { get; set; }

        public bool TrangThai { get; set; } = true;

        public ICollection<DangKyDeTai> DanhSachDangKy { get; set; } = new List<DangKyDeTai>();
    }
}