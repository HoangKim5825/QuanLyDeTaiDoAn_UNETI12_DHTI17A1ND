using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyDeTaiDoAn.Models
{
    public class PhanCongHuongDan
    {
        [Key]
        public int MaPhanCong { get; set; }

        public int MaDangKy { get; set; }
        [ForeignKey("MaDangKy")]
        public DangKyDeTai? DangKyDeTai { get; set; }

        public int MaGiangVien { get; set; }
        [ForeignKey("MaGiangVien")]
        public GiangVien? GiangVien { get; set; }

        [DataType(DataType.Date)]
        public DateTime NgayBatDau { get; set; }

        [DataType(DataType.Date)]
        public DateTime? NgayKetThuc { get; set; }   // Nullable - còn hiệu lực thì để trống

        // "DangHuongDan", "DaThayDoi", "KetThuc"
        [Required, StringLength(30)]
        public string TrangThai { get; set; } = "DangHuongDan";

        public string? GhiChu { get; set; }
    }
}