// Họ và tên: Hoàng Kim
// Mã sinh viên: 23203100056
// Nội dung thực hiện: Module 4 - Sinh viên xem GVHD hiện tại và lịch sử phân công của chính mình

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDeTaiDoAn.Data;
using QuanLyDeTaiDoAn.Filters;
using QuanLyDeTaiDoAn.Helpers;
using QuanLyDeTaiDoAn.ViewModels;

namespace QuanLyDeTaiDoAn.Controllers
{
    [YeuCauDangNhap]
    public class GVHDCuaToiController : Controller
    {
        private readonly ApplicationDbContext _context;
        public GVHDCuaToiController(ApplicationDbContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Vai trò và danh tính lấy từ Session do server set, KHÔNG nhận ID từ URL/form
            var vaiTro = HttpContext.Session.GetString(YeuCauDangNhapAttribute.SessionVaiTro);
            if (vaiTro != VaiTroTaiKhoan.SinhVien)
                return RedirectToAction("Index", "Home");

            int maTaiKhoan = HttpContext.Session.GetInt32(YeuCauDangNhapAttribute.SessionMaTaiKhoan)!.Value;

            var maSinhVien = await _context.SinhViens
                .Where(s => s.MaTaiKhoan == maTaiKhoan)
                .Select(s => (int?)s.MaSinhVien)
                .FirstOrDefaultAsync();
            if (maSinhVien == null)
                return View(new List<GVHDCuaToiItem>());   // tài khoản chưa có hồ sơ sinh viên

            var ds = await _context.DangKyDeTais
                .Where(dk => dk.MaSinhVien == maSinhVien)
                .OrderByDescending(dk => dk.NgayDangKy)
                .Select(dk => new GVHDCuaToiItem
                {
                    MaDangKy = dk.MaDangKy,
                    TenDeTai = dk.DeTai!.TenDeTai,
                    TenLinhVuc = dk.DeTai.LinhVuc!.TenLinhVuc,
                    TrangThai = dk.TrangThai,
                    TrangThaiThucHien = dk.TrangThaiThucHien,
                    GhiChuDuyet = dk.GhiChuDuyet,
                    TenGiangVienHienTai = dk.DanhSachPhanCong
                        .Where(p => p.TrangThai == TrangThaiPhanCong.DangHuongDan)
                        .Select(p => p.GiangVien!.HoTen)
                        .FirstOrDefault(),
                    LichSu = dk.DanhSachPhanCong
                        .OrderBy(p => p.NgayBatDau).ThenBy(p => p.MaPhanCong)
                        .Select(p => new LichSuPhanCongItem
                        {
                            MaPhanCong = p.MaPhanCong,
                            TenGiangVien = p.GiangVien!.HoTen,
                            HocVi = p.GiangVien.HocVi,
                            NgayBatDau = p.NgayBatDau,
                            NgayKetThuc = p.NgayKetThuc,
                            TrangThai = p.TrangThai,
                            GhiChu = p.GhiChu
                        }).ToList()
                })
                .ToListAsync();

            return View(ds);
        }
    }
}