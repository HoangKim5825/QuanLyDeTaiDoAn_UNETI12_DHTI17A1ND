// Họ và tên: Nguyễn Minh Đức
// Mã sinh viên: 23203100078
// Nội dung thực hiện: Module 1 - Bổ sung Display và thông báo lỗi tiếng Việt cho Entity LinhVuc
using System.ComponentModel.DataAnnotations;

namespace QuanLyDeTaiDoAn.Models
{
    public class LinhVuc
    {
        [Key]
        public int MaLinhVuc { get; set; }

        [Required(ErrorMessage = "Tên lĩnh vực không được để trống")]
        [StringLength(150, ErrorMessage = "Tên lĩnh vực tối đa 150 ký tự")]
        [Display(Name = "Tên lĩnh vực")]
        public string TenLinhVuc { get; set; } = string.Empty;

        // Không thêm StringLength/MaxLength ở đây: sẽ làm đổi kiểu cột và buộc phải tạo Migration mới.
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        // true = đang áp dụng, false = ngừng áp dụng
        [Display(Name = "Đang áp dụng")]
        public bool TrangThai { get; set; } = true;

        public ICollection<GiangVien> DanhSachGiangVien { get; set; } = new List<GiangVien>();
        public ICollection<DeTai> DanhSachDeTai { get; set; } = new List<DeTai>();
    }
}
