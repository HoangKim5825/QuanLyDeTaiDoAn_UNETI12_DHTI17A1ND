// Họ và tên: <Nguyen Quang Huy>
// Mã sinh viên: <232031000800>
// Nội dung thực hiện: ViewModel chi tiết đề tài kèm số chỗ tính toán (Module 2)
using QuanLyDeTaiDoAn.Constants;
using QuanLyDeTaiDoAn.Models;

namespace QuanLyDeTaiDoAn.ViewModels
{
    public class DeTaiDetailsViewModel
    {
        public DeTai DeTai { get; set; } = new();
        public int SoDaDangKy { get; set; }   // số đăng ký DaDuyet
        public int SoChoCon { get; set; }     // tính từ DB, không lưu

        public string TrangThaiHienThi =>
            DeTai.TrangThai == TrangThaiConst.DeTaiTT.MoDangKy && SoChoCon <= 0
                ? TrangThaiConst.DeTaiTT.DuSoLuong
                : DeTai.TrangThai;
    }
}
