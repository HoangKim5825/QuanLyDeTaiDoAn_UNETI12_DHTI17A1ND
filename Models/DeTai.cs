using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyDeTaiDoAn.Models
{
    public class DeTai
    {
        [Key]
        public int MaDeTai { get; set; }

        [Required, StringLength(250)]
        public string TenDeTai { get; set; } = string.Empty;

        public int MaLinhVuc { get; set; }
        [ForeignKey("MaLinhVuc")]
        public LinhVuc? LinhVuc { get; set; }

        public string? MoTa { get; set; }
        public string? YeuCau { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Phải lớn hơn 0")]
        public int SoSinhVienToiDa { get; set; }

        public int NamHoc { get; set; }

        [StringLength(10)]
        public string HocKy { get; set; } = string.Empty;

        // "MoDangKy", "DuSoLuong", "TamKhoa", "KetThuc"
        [Required, StringLength(30)]
        public string TrangThai { get; set; } = "MoDangKy";

        public ICollection<DangKyDeTai> DanhSachDangKy { get; set; } = new List<DangKyDeTai>();

        // Thuộc tính KHÔNG map vào DB, chỉ tính toán khi truy vấn
        [NotMapped]
        public int SoChoCon { get; set; }
    }
}