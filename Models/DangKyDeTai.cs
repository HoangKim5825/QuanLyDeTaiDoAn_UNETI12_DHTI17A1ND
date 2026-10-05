// Họ và tên: (Điền họ tên của bạn)
// Mã sinh viên: (Điền mã sinh viên của bạn)
// Nội dung thực hiện: Module 3 - Entity DangKyDeTai (Đăng ký đề tài)

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyDeTaiDoAn.Models
{
    // Ghi chú quan trọng (mục 7.6 đề bài):
    // DangKyDeTai là HỒ SƠ ĐĂNG KÝ, không đồng nghĩa với bản ghi phân công
    // giảng viên hướng dẫn. Vì vậy Entity này KHÔNG có cột MaGiangVien.
    // Việc phân công/đổi giảng viên được lưu riêng ở PhanCongHuongDan (Module 4)
    // để giữ được lịch sử thay đổi giảng viên qua thời gian.
    public class DangKyDeTai
    {
        [Key]
        public int MaDangKy { get; set; }

        [Required]
        [Display(Name = "Sinh viên")]
        public string MaSinhVien { get; set; }

        [ForeignKey(nameof(MaSinhVien))]
        public SinhVien SinhVien { get; set; }

        [Required]
        [Display(Name = "Đề tài")]
        public int MaDeTai { get; set; }

        [ForeignKey(nameof(MaDeTai))]
        public DeTai DeTai { get; set; }

        [Required]
        [Display(Name = "Ngày đăng ký")]
        public DateTime NgayDangKy { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Lý do đăng ký")]
        [StringLength(500)]
        public string LyDoDangKy { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } // Chờ duyệt / Đã duyệt / Từ chối / Đã hủy

        [DataType(DataType.MultilineText)]
        [Display(Name = "Ghi chú duyệt")]
        [StringLength(500)]
        public string GhiChuDuyet { get; set; }

        public ICollection<PhanCongHuongDan> PhanCongHuongDans { get; set; } = new List<PhanCongHuongDan>();
    }
}
