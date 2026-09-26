using System.ComponentModel.DataAnnotations;

namespace QuanLyDeTaiDoAn.Models
{
    public class LinhVuc
    {
        [Key]
        public int MaLinhVuc { get; set; }

        [Required, StringLength(150)]
        public string TenLinhVuc { get; set; } = string.Empty;

        public string? MoTa { get; set; }

        public bool TrangThai { get; set; } = true;

        public ICollection<GiangVien> DanhSachGiangVien { get; set; } = new List<GiangVien>();
        public ICollection<DeTai> DanhSachDeTai { get; set; } = new List<DeTai>();
    }
}