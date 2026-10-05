// Họ và tên: (Điền họ tên của bạn)
// Mã sinh viên: (Điền mã sinh viên của bạn)
// Nội dung thực hiện: Module 3 - ViewModel hỗ trợ hiển thị đăng ký đề tài

namespace QuanLyDeTaiDoAn.ViewModels
{
    /// <summary>
    /// Dùng để hiển thị 1 dòng trong danh sách "Đề tài đang mở đăng ký"
    /// (DangKyDeTaiController.DanhSachDeTai). SoChoCon được TÍNH bằng LINQ,
    /// không lấy trực tiếp từ cột nào trong bảng DeTai.
    /// </summary>
    public class DeTaiKhaDungViewModel
    {
        public int MaDeTai { get; set; }
        public string TenDeTai { get; set; }
        public string TenLinhVuc { get; set; }
        public string NamHoc { get; set; }
        public string HocKy { get; set; }
        public int SoSinhVienToiDa { get; set; }
        public int SoChoCon { get; set; }
        public string TrangThai { get; set; }
    }

    /// <summary>
    /// Dùng để hiển thị 1 dòng trong "Theo dõi kết quả đăng ký" của chính sinh viên
    /// (DangKyDeTaiController.Index). Gộp thông tin đăng ký + đề tài + GVHD hiện tại.
    /// </summary>
    public class KetQuaDangKyViewModel
    {
        public int MaDangKy { get; set; }
        public string TenDeTai { get; set; }
        public string TenLinhVuc { get; set; }
        public DateTime NgayDangKy { get; set; }
        public string TrangThai { get; set; }
        public string GhiChuDuyet { get; set; }
        public string TenGiangVienHuongDan { get; set; } // null nếu chưa được phân công
    }
}
