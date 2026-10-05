using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyDeTaiDoAn.Models
{
    // Ghi chú: Entity này thuộc phạm vi phụ trách chính của Module 2
    // (Quản lý đề tài - Tìm kiếm - Lọc - Sắp xếp - Phân trang).
    // Module 3 đọc dữ liệu đề tài để sinh viên đăng ký (DangKyDeTaiController).
    //
    // QUAN TRỌNG: KHÔNG lưu "SoChoCon" thành cột trong bảng này.
    // Số chỗ còn phải luôn được TÍNH bằng LINQ từ SoSinhVienToiDa trừ đi
    // số DangKyDeTai đang ở trạng thái hợp lệ (xem AppConstants.DangKy_TrangThaiHopLeDeTinhCho
    // và DangKyDeTaiController.TinhSoChoCon()).
    public class DeTai
    {
        [Key]
        public int MaDeTai { get; set; }

        [Required(ErrorMessage = "Tên đề tài không được để trống")]
        [StringLength(200)]
        [Display(Name = "Tên đề tài")]
        public string TenDeTai { get; set; }

        [Required]
        [Display(Name = "Lĩnh vực")]
        public int MaLinhVuc { get; set; }

        [ForeignKey(nameof(MaLinhVuc))]
        public LinhVuc LinhVuc { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Mô tả")]
        public string MoTa { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Yêu cầu")]
        public string YeuCau { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số sinh viên tối đa phải > 0")]
        [Display(Name = "Số SV tối đa")]
        public int SoSinhVienToiDa { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Năm học")]
        public string NamHoc { get; set; } // vd "2025-2026"

        [Required]
        [StringLength(10)]
        [Display(Name = "Học kỳ")]
        public string HocKy { get; set; } // vd "1", "2", "He"

        [Required]
        [StringLength(20)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } // Mở đăng ký / Đủ số lượng / Tạm khóa / Kết thúc

        public ICollection<DangKyDeTai> DangKyDeTais { get; set; } = new List<DangKyDeTai>();
    }
}
