// Họ và tên: Hoàng Kim
// Mã sinh viên: <23203100056>
// Nội dung thực hiện: Module 4 - ViewModel danh sách đăng ký chờ duyệt (tìm kiếm, lọc, phân trang)

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuanLyDeTaiDoAn.ViewModels
{
    public class DangKyDuyetItem
    {
        public int MaDangKy { get; set; }
        public int MaSinhVien { get; set; }
        public string TenSinhVien { get; set; } = string.Empty;
        public string TenDeTai { get; set; } = string.Empty;
        public string TenLinhVuc { get; set; } = string.Empty;
        public DateTime NgayDangKy { get; set; }
        public string? LyDoDangKy { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public string? GhiChuDuyet { get; set; }

        public string TrangThaiThucHien { get; set; } = string.Empty;
        public int SoChoCon { get; set; }   // tính lúc truy vấn, không lưu

        // Phân công đang hiệu lực (null nếu chưa có GVHD)
        public int? MaPhanCongHienTai { get; set; }
        public string? TenGiangVienHienTai { get; set; }
    }

    public class DanhSachChoDuyetViewModel
    {
        public List<DangKyDuyetItem> Items { get; set; } = new();

        // Điều kiện tìm kiếm / lọc (giữ lại khi chuyển trang)
        public string? TuKhoa { get; set; }
        public int? MaLinhVuc { get; set; }
        public string TrangThai { get; set; } = "ChoDuyet";   // "TatCa" = không lọc
        public SelectList LinhVucs { get; set; } = new SelectList(Array.Empty<object>());

        // Phân trang
        public int Page { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int TotalItems { get; set; }
    }

    // Dùng chung cho form Phân công (MaPhanCongCu = null) và Thay đổi GVHD (MaPhanCongCu có giá trị)
    public class PhanCongViewModel
    {
        public int MaDangKy { get; set; }
        public int? MaPhanCongCu { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn giảng viên hướng dẫn")]
        [Display(Name = "Giảng viên hướng dẫn")]
        public int? MaGiangVien { get; set; }

        [StringLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự")]
        [Display(Name = "Ghi chú / Lý do")]
        public string? GhiChu { get; set; }

        // ===== Chỉ để hiển thị, nạp lại từ DB mỗi lần trả View; không bind, không validate =====
        [BindNever, ValidateNever] public string TenSinhVien { get; set; } = string.Empty;
        [BindNever, ValidateNever] public string TenDeTai { get; set; } = string.Empty;
        [BindNever, ValidateNever] public string TenLinhVuc { get; set; } = string.Empty;
        [BindNever, ValidateNever] public string? TenGiangVienHienTai { get; set; }
        [BindNever, ValidateNever] public DateTime? NgayBatDauHienTai { get; set; }
        [BindNever, ValidateNever] public List<GiangVienTaiItem> GiangVienPhuHop { get; set; } = new();

        public bool LaThayDoi => MaPhanCongCu.HasValue;
    }

    // ===== Lịch sử phân công của một đăng ký =====
    public class LichSuPhanCongItem
    {
        public int MaPhanCong { get; set; }
        public string TenGiangVien { get; set; } = string.Empty;
        public string? HocVi { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public string? GhiChu { get; set; }
    }

    public class LichSuPhanCongViewModel
    {
        public int MaDangKy { get; set; }
        public string TenSinhVien { get; set; } = string.Empty;
        public string TenDeTai { get; set; } = string.Empty;
        public string TenLinhVuc { get; set; } = string.Empty;
        public string TrangThaiDangKy { get; set; } = string.Empty;
        public List<LichSuPhanCongItem> DanhSach { get; set; } = new();
    }
    public class GVHDCuaToiItem
    {
        public int MaDangKy { get; set; }
        public string TenDeTai { get; set; } = string.Empty;
        public string TenLinhVuc { get; set; } = string.Empty;
        public string TrangThai { get; set; } = string.Empty;
        public string TrangThaiThucHien { get; set; } = string.Empty;
        public string? GhiChuDuyet { get; set; }
        public string? TenGiangVienHienTai { get; set; }
        public List<LichSuPhanCongItem> LichSu { get; set; } = new();
    }
}
