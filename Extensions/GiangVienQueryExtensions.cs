//Họ và tên: <Nguyen Quang Huy>
//Mã sinh viên: <23203100080>
// Nội dung thực hiện: Công thức tính tải hướng dẫn hiện tại của giảng viên dùng chung (Module 2)
using QuanLyDeTaiDoAn.Constants;
using QuanLyDeTaiDoAn.Models;
using QuanLyDeTaiDoAn.ViewModels;

namespace QuanLyDeTaiDoAn.Extensions
{
    public static class GiangVienQueryExtensions
    {
        // CÔNG THỨC DUY NHẤT (M4 dùng chung khi phân công):
        // TaiHienTai = số PhanCongHuongDan có TrangThai = "DangHuongDan"
        // (phân công DaThayDoi/KetThuc không tính)
        public static IQueryable<GiangVienListItem> ToListItems(this IQueryable<GiangVien> query)
        {
            return query.Select(g => new GiangVienListItem
            {
                MaGiangVien = g.MaGiangVien,
                HoTen = g.HoTen,
                HocVi = g.HocVi,
                Email = g.Email,
                SoDienThoai = g.SoDienThoai,
                MaLinhVuc = g.MaLinhVuc,
                TenLinhVuc = g.LinhVuc!.TenLinhVuc,
                SoSinhVienHuongDanToiDa = g.SoSinhVienHuongDanToiDa,
                TrangThai = g.TrangThai,
                TaiHienTai = g.DanhSachPhanCong
                    .Count(p => p.TrangThai == TrangThaiConst.PhanCongTT.DangHuongDan),
                TongPhanCong = g.DanhSachPhanCong.Count()
            });
        }
    }
}
