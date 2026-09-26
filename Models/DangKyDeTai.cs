using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyDeTaiDoAn.Models
{
    public class DangKyDeTai
    {
        [Key]
        public int MaDangKy { get; set; }

        public int MaSinhVien { get; set; }
        [ForeignKey("MaSinhVien")]
        public SinhVien? SinhVien { get; set; }

        public int MaDeTai { get; set; }
        [ForeignKey("MaDeTai")]
        public DeTai? DeTai { get; set; }

        public DateTime NgayDangKy { get; set; }

        public string? LyDoDangKy { get; set; }

        // "ChoDuyet", "DaDuyet", "TuChoi", "DaHuy"
        [Required, StringLength(30)]
        public string TrangThai { get; set; } = "ChoDuyet";

        public string? GhiChuDuyet { get; set; }

        public ICollection<PhanCongHuongDan> DanhSachPhanCong { get; set; } = new List<PhanCongHuongDan>();
    }
}