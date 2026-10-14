// Họ và tên: <ĐIỀN TÊN>
// Mã sinh viên: <ĐIỀN MÃ SV>
// Nội dung thực hiện: Module 3 - ViewModel đăng ký đề tài, danh sách và chi tiết đăng ký của sinh viên
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace QuanLyDeTaiDoAn.ViewModels
{
    public class DangKyDeTaiCreateViewModel
    {
        public int MaDeTai { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập lý do đăng ký")]
        [StringLength(1000, ErrorMessage = "Lý do tối đa 1000 ký tự")]
        [Display(Name = "Lý do đăng ký")]
        public string? LyDoDangKy { get; set; }

        // Chỉ hiển thị, nạp lại từ DB mỗi lần trả View
        [BindNever, ValidateNever] public string TenSinhVien { get; set; } = string.Empty;
        [BindNever, ValidateNever] public string TenDeTai { get; set; } = string.Empty;
        [BindNever, ValidateNever] public string TenLinhVuc { get; set; } = string.Empty;
        [BindNever, ValidateNever] public string? MoTa { get; set; }
        [BindNever, ValidateNever] public string? YeuCau { get; set; }
        [BindNever, ValidateNever] public int NamHoc { get; set; }
        [BindNever, ValidateNever] public string HocKy { get; set; } = string.Empty;
        [BindNever, ValidateNever] public int SoSinhVienToiDa { get; set; }
        [BindNever, ValidateNever] public int SoChoCon { get; set; }
        [BindNever, ValidateNever] public List<string> Loi { get; set; } = new();
    }

    public class DangKyCuaToiItem
    {
        public int MaDangKy { get; set; }
        public int MaDeTai { get; set; }
        public string TenDeTai { get; set; } = string.Empty;
        public string TenLinhVuc { get; set; } = string.Empty;
        public int NamHoc { get; set; }
        public string HocKy { get; set; } = string.Empty;
        public DateTime NgayDangKy { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public string TrangThaiThucHien { get; set; } = string.Empty;
        public string? GhiChuDuyet { get; set; }
        public string? TenGiangVienHienTai { get; set; }
    }

    public class DangKyChiTietViewModel
    {
        public int MaDangKy { get; set; }
        public int MaDeTai { get; set; }
        public string TenSinhVien { get; set; } = string.Empty;
        public string TenDeTai { get; set; } = string.Empty;
        public string TenLinhVuc { get; set; } = string.Empty;
        public int NamHoc { get; set; }
        public string HocKy { get; set; } = string.Empty;
        public DateTime NgayDangKy { get; set; }
        public string? LyDoDangKy { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public string TrangThaiThucHien { get; set; } = string.Empty;
        public string? GhiChuDuyet { get; set; }
        public string? TenGiangVienHienTai { get; set; }
        public List<LichSuPhanCongItem> LichSu { get; set; } = new();
    }
}
