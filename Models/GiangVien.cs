using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyDeTaiDoAn.Models
{
    public class GiangVien
    {
        [Key]
        public int MaGiangVien { get; set; }

        [Required, StringLength(100)]
        public string HoTen { get; set; } = string.Empty;

        [StringLength(50)]
        public string? HocVi { get; set; }

        [Required, EmailAddress, StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Phone, StringLength(15)]
        public string? SoDienThoai { get; set; }

        public int MaLinhVuc { get; set; }
        [ForeignKey("MaLinhVuc")]
        public LinhVuc? LinhVuc { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Phải lớn hơn 0")]
        public int SoSinhVienHuongDanToiDa { get; set; }

        // "DangCongTac" hoặc "TamNgungHuongDan"
        [Required, StringLength(30)]
        public string TrangThai { get; set; } = "DangCongTac";

        public ICollection<PhanCongHuongDan> DanhSachPhanCong { get; set; } = new List<PhanCongHuongDan>();
    }
}