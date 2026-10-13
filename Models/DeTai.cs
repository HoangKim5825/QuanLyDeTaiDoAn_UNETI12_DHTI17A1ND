// Họ và tên: <Nguyen Quang Huy>
// Mã sinh viên: <23213100080>
// Nội dung thực hiện: Bổ sung Display và thông báo lỗi tiếng Việt cho Entity DeTai (Module 2)
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyDeTaiDoAn.Models
{
    public class DeTai
    {
        [Key]
        public int MaDeTai { get; set; }

        [Required(ErrorMessage = "Tên đề tài không được để trống")]
        [StringLength(250, ErrorMessage = "Tên đề tài tối đa 250 ký tự")]
        [Display(Name = "Tên đề tài")]
        public string TenDeTai { get; set; } = string.Empty;

        [Display(Name = "Lĩnh vực")]
        public int MaLinhVuc { get; set; }
        [ForeignKey("MaLinhVuc")]
        public LinhVuc? LinhVuc { get; set; }

        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [Display(Name = "Yêu cầu")]
        public string? YeuCau { get; set; }

        // File mô tả đề tài (nâng cao). Cả hai cột đều nullable nên dữ liệu cũ không bị ảnh hưởng.
        [StringLength(255)]
        [Display(Name = "File mô tả")]
        public string? TenFileMoTa { get; set; }   // tên gốc, để hiển thị khi tải về

        [StringLength(100)]
        public string? TenFileLuu { get; set; }    // tên ngẫu nhiên đã lưu trên đĩa

        [Range(1, int.MaxValue, ErrorMessage = "Số sinh viên tối đa phải lớn hơn 0")]
        [Display(Name = "Số sinh viên tối đa")]
        public int SoSinhVienToiDa { get; set; }

        [Display(Name = "Năm học")]
        public int NamHoc { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn học kỳ")]
        [StringLength(10)]
        [Display(Name = "Học kỳ")]
        public string HocKy { get; set; } = string.Empty;

        // "MoDangKy", "DuSoLuong", "TamKhoa", "KetThuc"
        [Required(ErrorMessage = "Vui lòng chọn trạng thái")]
        [StringLength(30)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "MoDangKy";

        public ICollection<DangKyDeTai> DanhSachDangKy { get; set; } = new List<DangKyDeTai>();

        // Thuộc tính KHÔNG map vào DB, chỉ tính toán khi truy vấn
        [NotMapped]
        public int SoChoCon { get; set; }
    }
}
