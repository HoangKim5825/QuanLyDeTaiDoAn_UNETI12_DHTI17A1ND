using System.ComponentModel.DataAnnotations;

namespace QuanLyDeTaiDoAn.Models
{
    public class TaiKhoan
    {
        [Key]
        public int MaTaiKhoan { get; set; }

        [Required, StringLength(50)]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string MatKhau { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string HoTen { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(100)]
        public string Email { get; set; } = string.Empty;

        // "Admin" hoặc "SinhVien"
        [Required, StringLength(20)]
        public string VaiTro { get; set; } = string.Empty;

        // true = đang hoạt động, false = bị khóa
        public bool TrangThai { get; set; } = true;

        public SinhVien? SinhVien { get; set; }
    }
}