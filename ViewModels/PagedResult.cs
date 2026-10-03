// Họ và tên: <Nguyen Quang Huy>
// Mã sinh viên: <23203100080>
// Nội dung thực hiện: ViewModel phân trang và dòng danh sách đề tài (Module 2)
using QuanLyDeTaiDoAn.Constants;

namespace QuanLyDeTaiDoAn.ViewModels
{
    public class PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; set; } = new List<T>();
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 8;
        public int TotalItems { get; set; }

        public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalItems / (double)PageSize);
        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < TotalPages;
    }

    // Một dòng trong danh sách đề tài. SoChoCon được TÍNH trong truy vấn, không lưu DB.
    public class DeTaiListItem
    {
        public int MaDeTai { get; set; }
        public string TenDeTai { get; set; } = string.Empty;
        public int MaLinhVuc { get; set; }
        public string TenLinhVuc { get; set; } = string.Empty;
        public int NamHoc { get; set; }
        public string HocKy { get; set; } = string.Empty;
        public int SoSinhVienToiDa { get; set; }
        public int SoDaDangKy { get; set; }   // số đăng ký DaDuyet
        public int SoChoCon { get; set; }
        public string TrangThai { get; set; } = string.Empty;

        // Đề tài đang mở nhưng hết chỗ thì hiển thị là "Đủ số lượng"
        public string TrangThaiHienThi =>
            TrangThai == TrangThaiConst.DeTaiTT.MoDangKy && SoChoCon <= 0
                ? TrangThaiConst.DeTaiTT.DuSoLuong
                : TrangThai;
    }
}
