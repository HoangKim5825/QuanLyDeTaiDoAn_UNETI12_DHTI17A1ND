// Họ và tên: <ĐIỀN TÊN>
// Mã sinh viên: <ĐIỀN MÃ SV>
// Nội dung thực hiện: Module 3 - ViewModel đăng ký đề tài và theo dõi kết quả đăng ký

using System.ComponentModel.DataAnnotations;
using QuanLyDeTaiDoAn.Helpers;

namespace QuanLyDeTaiDoAn.ViewModels
{
    // Form đăng ký: sinh viên CHỈ nhập lý do. MaSinhVien lấy từ Session, NgayDangKy do hệ thống gán.
    public class DangKyCreateViewModel
    {
        public int MaDeTai { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập lý do đăng ký")]
        [StringLength(1000, ErrorMessage = "Lý do tối đa 1000 ký tự")]
        [Display(Name = "Lý do đăng ký")]
        public string? LyDoDangKy { get; set; }

        // ===== Chỉ để hiển thị, nạp lại từ DB mỗi lần trả View =====
        public string? TenDeTai { get; set; }
        public string? TenLinhVuc { get; set; }
        public int NamHoc { get; set; }
        public string? HocKy { get; set; }
        public string? TrangThaiDeTai { get; set; }
        public int SoSinhVienToiDa { get; set; }
        public int SoChoCon { get; set; }       // tính từ DB, không lưu
        public string? MoTa { get; set; }
        public string? YeuCau { get; set; }
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
        public string? LyDoDangKy { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public string TrangThaiThucHien { get; set; } = string.Empty;
        public string? GhiChuDuyet { get; set; }
        public string? TenGiangVienHienTai { get; set; }   // null nếu chưa được phân công

        public bool CoTheHuy => TrangThai == TrangThaiDangKy.ChoDuyet;

        public string NhanTrangThai => TrangThai switch
        {
            TrangThaiDangKy.ChoDuyet => "Chờ duyệt",
            TrangThaiDangKy.DaDuyet => "Đã duyệt",
            TrangThaiDangKy.TuChoi => "Từ chối",
            TrangThaiDangKy.DaHuy => "Đã hủy",
            _ => TrangThai
        };

        public string MauTrangThai => TrangThai switch
        {
            TrangThaiDangKy.ChoDuyet => "text-bg-warning",
            TrangThaiDangKy.DaDuyet => "text-bg-success",
            TrangThaiDangKy.TuChoi => "text-bg-danger",
            _ => "text-bg-secondary"
        };

        public string NhanTienDo => TrangThaiThucHien switch
        {
            TienDoThucHien.DangThucHien => "Đang thực hiện",
            TienDoThucHien.HoanThanh => "Hoàn thành",
            _ => "Chưa bắt đầu"
        };
    }

    public class DangKyCuaToiListViewModel
    {
        public string? TrangThai { get; set; }
        public List<DangKyCuaToiItem> Items { get; set; } = new();
    }

    public class DangKyChiTietViewModel
    {
        public DangKyCuaToiItem DangKy { get; set; } = new();
        public List<LichSuPhanCongItem> LichSu { get; set; } = new();   // dùng lại ViewModel của Module 4
    }
}
