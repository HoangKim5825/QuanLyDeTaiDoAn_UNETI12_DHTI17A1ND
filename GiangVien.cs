using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyDeTaiDoAn.Models
{
    // Ghi chú: Entity này thuộc phạm vi phụ trách chính của Module 2
    // (Quản lý giảng viên). Module 3 chỉ đọc dữ liệu để hiển thị giảng viên
    // hướng dẫn hiện tại của sinh viên (qua DangKyDeTai -> PhanCongHuongDan -> GiangVien).
    public class GiangVien
    {
        [Key]
        public int MaGiangVien { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100)]
        [Display(Name = "Họ tên")]
        public string HoTen { get; set; }

        [StringLength(50)]
        [Display(Name = "Học vị")]
        public string HocVi { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100)]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(15)]
        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; }

        [Required]
        [Display(Name = "Lĩnh vực")]
        public int MaLinhVuc { get; set; }

        [ForeignKey(nameof(MaLinhVuc))]
        public LinhVuc LinhVuc { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số sinh viên hướng dẫn tối đa phải > 0")]
        [Display(Name = "Số SV hướng dẫn tối đa")]
        public int SoSinhVienHuongDanToiDa { get; set; }

        [Required]
        [StringLength(30)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } // "Đang công tác" / "Tạm ngừng hướng dẫn"

        public ICollection<PhanCongHuongDan> PhanCongHuongDans { get; set; } = new List<PhanCongHuongDan>();
    }
}
