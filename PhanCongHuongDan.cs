using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyDeTaiDoAn.Models
{
    // Ghi chú: Entity này thuộc phạm vi phụ trách chính của Module 4
    // (Duyệt đăng ký - Phân công/thay đổi GVHD). Module 3 chỉ ĐỌC dữ liệu
    // (bản ghi có TrangThai = "Đang hướng dẫn") để hiển thị cho sinh viên
    // biết ai đang là giảng viên hướng dẫn của mình.
    public class PhanCongHuongDan
    {
        [Key]
        public int MaPhanCong { get; set; }

        [Required]
        [Display(Name = "Đăng ký")]
        public int MaDangKy { get; set; }

        [ForeignKey(nameof(MaDangKy))]
        public DangKyDeTai DangKyDeTai { get; set; }

        [Required]
        [Display(Name = "Giảng viên")]
        public int MaGiangVien { get; set; }

        [ForeignKey(nameof(MaGiangVien))]
        public GiangVien GiangVien { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày bắt đầu")]
        public DateTime NgayBatDau { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày kết thúc")]
        public DateTime? NgayKetThuc { get; set; } // Nullable: để trống khi đang hiệu lực

        [Required]
        [StringLength(20)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } // Đang hướng dẫn / Đã thay đổi / Kết thúc

        [StringLength(300)]
        [Display(Name = "Ghi chú")]
        public string GhiChu { get; set; }
    }
}
