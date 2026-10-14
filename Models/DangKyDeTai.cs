using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyDeTaiDoAn.Models
{
    public class DangKyDeTai
    {
        [Key]
        [Display(Name = "Mã đăng ký")]
        public int MaDangKy { get; set; }

        public int MaSinhVien { get; set; }
        [ForeignKey("MaSinhVien")]
        public SinhVien? SinhVien { get; set; }

        public int MaDeTai { get; set; }
        [ForeignKey("MaDeTai")]
        public DeTai? DeTai { get; set; }

        [Display(Name = "Ngày đăng ký")]
        public DateTime NgayDangKy { get; set; }

        [Display(Name = "Lý do đăng ký")]
        public string? LyDoDangKy { get; set; }

        // "ChoDuyet", "DaDuyet", "TuChoi", "DaHuy"
        [Required, StringLength(30)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "ChoDuyet";

        // "ChuaBatDau", "DangThucHien", "HoanThanh" - chỉ Admin cập nhật
        [Required, StringLength(30)]
        [Display(Name = "Tiến độ")]
        public string TrangThaiThucHien { get; set; } = "ChuaBatDau";

        [Display(Name = "Ghi chú duyệt")]
        public string? GhiChuDuyet { get; set; }

        public ICollection<PhanCongHuongDan> DanhSachPhanCong { get; set; } = new List<PhanCongHuongDan>();
    }
}