// Họ và tên: <Nguyen Quang Huy>
// Mã sinh viên: <23203100080>
// Nội dung thực hiện: Bổ sung Display và thông báo lỗi tiếng Việt cho Entity GiangVien (Module 2)
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyDeTaiDoAn.Models
{
    public class GiangVien
    {
        [Key]
        public int MaGiangVien { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Học vị tối đa 50 ký tự")]
        [Display(Name = "Học vị")]
        public string? HocVi { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100, ErrorMessage = "Email tối đa 100 ký tự")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(15, ErrorMessage = "Số điện thoại tối đa 15 ký tự")]
        [Display(Name = "Số điện thoại")]
        public string? SoDienThoai { get; set; }

        [Display(Name = "Lĩnh vực")]
        public int MaLinhVuc { get; set; }
        [ForeignKey("MaLinhVuc")]
        public LinhVuc? LinhVuc { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số sinh viên hướng dẫn tối đa phải lớn hơn 0")]
        [Display(Name = "Số sinh viên hướng dẫn tối đa")]
        public int SoSinhVienHuongDanToiDa { get; set; }

        // "DangCongTac" hoặc "TamNgungHuongDan"
        [Required(ErrorMessage = "Vui lòng chọn trạng thái")]
        [StringLength(30)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "DangCongTac";

        public ICollection<PhanCongHuongDan> DanhSachPhanCong { get; set; } = new List<PhanCongHuongDan>();
    }
}
