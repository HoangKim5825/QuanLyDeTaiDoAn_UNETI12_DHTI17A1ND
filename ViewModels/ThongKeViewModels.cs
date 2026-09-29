// Họ và tên: Hoàng Kim
// Mã sinh viên: <23203100056>
// Nội dung thực hiện: Module 4 - ViewModel cho Dashboard và Thống kê

namespace QuanLyDeTaiDoAn.ViewModels
{
    // ===== Dashboard: 10 chỉ số theo mục 8.7 =====
    public class DashboardViewModel
    {
        public int TongLinhVuc { get; set; }
        public int TongDeTai { get; set; }
        public int TongSinhVien { get; set; }
        public int TongGiangVien { get; set; }
        public int SoDangKyChoDuyet { get; set; }
        public int SoDangKyDaDuyet { get; set; }
        public int SoDeTaiConCho { get; set; }
        public int SoDeTaiDuSoLuong { get; set; }
        public int SoSinhVienCoGVHD { get; set; }
        public int SoGiangVienDatGioiHan { get; set; }
    }

    // Dòng dữ liệu chung: nhãn + số lượng (dùng cho GroupBy)
    public class ThongKeItem
    {
        public string Nhan { get; set; } = string.Empty;
        public int SoLuong { get; set; }
    }

    // Số sinh viên và số chỗ còn của từng đề tài
    public class DeTaiChoConItem
    {
        public int MaDeTai { get; set; }
        public string TenDeTai { get; set; } = string.Empty;
        public string TenLinhVuc { get; set; } = string.Empty;
        public string TrangThai { get; set; } = string.Empty;
        public int SoSinhVienToiDa { get; set; }
        public int SoDaDuyet { get; set; }
        public int SoChoCon => SoSinhVienToiDa - SoDaDuyet; // luôn tính, không lưu
    }

    // Tải hướng dẫn của từng giảng viên
    public class GiangVienTaiItem
    {
        public int MaGiangVien { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string TenLinhVuc { get; set; } = string.Empty;
        public int SoDangHuongDan { get; set; }
        public int SoToiDa { get; set; }
        public double TyLeSuDung => SoToiDa > 0 ? Math.Round(SoDangHuongDan * 100.0 / SoToiDa, 1) : 0;
    }

    public class ThongKeViewModel
    {
        public List<ThongKeItem> DeTaiTheoLinhVuc { get; set; } = new();
        public List<ThongKeItem> DangKyTheoTrangThai { get; set; } = new();
        public List<DeTaiChoConItem> DeTaiChoCon { get; set; } = new();
        public List<GiangVienTaiItem> TaiGiangVien { get; set; } = new();
        public List<GiangVienTaiItem> GiangVienNhieuNhat { get; set; } = new();
        public List<ThongKeItem> DangKyTheoThang { get; set; } = new();
        public List<ThongKeItem> DeTaiTheoNamHocHocKy { get; set; } = new();
    }
}