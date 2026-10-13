// Họ và tên: Hoàng Kim
// Mã sinh viên: <23203100056>
// Nội dung thực hiện: Module 4 - Dashboard và Thống kê bằng LINQ
// (Count, Any, Where, GroupBy, OrderBy). Số liệu luôn truy vấn trực tiếp DB.

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDeTaiDoAn.Data;
using QuanLyDeTaiDoAn.Filters;
using QuanLyDeTaiDoAn.Services;
using QuanLyDeTaiDoAn.ViewModels;

namespace QuanLyDeTaiDoAn.Controllers
{
    [YeuCauAdmin]
    public class ThongKeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly XuatFileService _xuat;

        public ThongKeController(ApplicationDbContext context, XuatFileService xuat)
        {
            _context = context;
            _xuat = xuat;
        }

        // ===== DASHBOARD =====
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            // Số chỗ còn = SoSinhVienToiDa - số đăng ký "DaDuyet" (tính lúc truy vấn)
            var deTaiVaChoCon = _context.DeTais.Select(d => new
            {
                d.TrangThai,
                SoChoCon = d.SoSinhVienToiDa - d.DanhSachDangKy.Count(dk => dk.TrangThai == "DaDuyet")
            });

            var vm = new DashboardViewModel
            {
                TongLinhVuc = await _context.LinhVucs.CountAsync(),
                TongDeTai = await _context.DeTais.CountAsync(),
                TongSinhVien = await _context.SinhViens.CountAsync(),
                TongGiangVien = await _context.GiangViens.CountAsync(),

                SoDangKyChoDuyet = await _context.DangKyDeTais.CountAsync(dk => dk.TrangThai == "ChoDuyet"),
                SoDangKyDaDuyet = await _context.DangKyDeTais.CountAsync(dk => dk.TrangThai == "DaDuyet"),

                SoDeTaiConCho = await deTaiVaChoCon.CountAsync(x => x.TrangThai == "MoDangKy" && x.SoChoCon > 0),
                SoDeTaiDuSoLuong = await deTaiVaChoCon.CountAsync(x => x.SoChoCon <= 0),

                // Sinh viên đang có GVHD = số sinh viên khác nhau có phân công "DangHuongDan"
                SoSinhVienCoGVHD = await _context.PhanCongHuongDans
                    .Where(p => p.TrangThai == "DangHuongDan")
                    .Select(p => p.DangKyDeTai!.MaSinhVien)
                    .Distinct()
                    .CountAsync(),

                // Giảng viên đạt giới hạn: tải hiện tại >= SoSinhVienHuongDanToiDa
                SoGiangVienDatGioiHan = await _context.GiangViens.CountAsync(g =>
                    g.DanhSachPhanCong.Count(p => p.TrangThai == "DangHuongDan") >= g.SoSinhVienHuongDanToiDa)
            };

            return View(vm);
        }

        // ===== THỐNG KÊ =====
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vm = new ThongKeViewModel();

            // 1. Số đề tài theo từng lĩnh vực (GroupBy)
            vm.DeTaiTheoLinhVuc = await _context.DeTais
                .GroupBy(d => d.LinhVuc!.TenLinhVuc)
                .Select(g => new ThongKeItem { Nhan = g.Key, SoLuong = g.Count() })
                .OrderByDescending(x => x.SoLuong)
                .ToListAsync();

            // 2. Số đăng ký theo trạng thái (GroupBy)
            var dkTheoTrangThai = await _context.DangKyDeTais
                .GroupBy(dk => dk.TrangThai)
                .Select(g => new { TrangThai = g.Key, SoLuong = g.Count() })
                .ToListAsync();
            vm.DangKyTheoTrangThai = dkTheoTrangThai
                .Select(x => new ThongKeItem { Nhan = NhanTrangThaiDangKy(x.TrangThai), SoLuong = x.SoLuong })
                .OrderByDescending(x => x.SoLuong)
                .ToList();

            // 3 + 4. Số sinh viên (đã duyệt) và số chỗ còn của từng đề tài
            vm.DeTaiChoCon = await _context.DeTais
                .Select(d => new DeTaiChoConItem
                {
                    MaDeTai = d.MaDeTai,
                    TenDeTai = d.TenDeTai,
                    TenLinhVuc = d.LinhVuc!.TenLinhVuc,
                    TrangThai = d.TrangThai,
                    SoSinhVienToiDa = d.SoSinhVienToiDa,
                    SoDaDuyet = d.DanhSachDangKy.Count(dk => dk.TrangThai == "DaDuyet")
                })
                .OrderBy(x => x.TenDeTai)
                .ToListAsync();

            // 5 + 6 + 9. Tải từng giảng viên, giảng viên nhiều nhất, tỷ lệ sử dụng
            // Chỉ đếm phân công "DangHuongDan" (DaThayDoi/KetThuc không tính)
            vm.TaiGiangVien = await _context.GiangViens
                .Select(g => new GiangVienTaiItem
                {
                    MaGiangVien = g.MaGiangVien,
                    HoTen = g.HoTen,
                    TenLinhVuc = g.LinhVuc!.TenLinhVuc,
                    SoToiDa = g.SoSinhVienHuongDanToiDa,
                    SoDangHuongDan = g.DanhSachPhanCong.Count(p => p.TrangThai == "DangHuongDan")
                })
                .OrderByDescending(x => x.SoDangHuongDan)
                .ThenBy(x => x.HoTen)
                .ToListAsync();

            var max = vm.TaiGiangVien.Any() ? vm.TaiGiangVien.Max(x => x.SoDangHuongDan) : 0;
            vm.GiangVienNhieuNhat = max > 0
                ? vm.TaiGiangVien.Where(x => x.SoDangHuongDan == max).ToList()   // có thể đồng hạng
                : new List<GiangVienTaiItem>();

            // 7. Số đăng ký theo tháng (GroupBy năm + tháng)
            var theoThang = await _context.DangKyDeTais
                .GroupBy(dk => new { dk.NgayDangKy.Year, dk.NgayDangKy.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, SoLuong = g.Count() })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();
            vm.DangKyTheoThang = theoThang
                .Select(x => new ThongKeItem { Nhan = $"{x.Month:00}/{x.Year}", SoLuong = x.SoLuong })
                .ToList();

            // 8. Số đề tài theo năm học / học kỳ (GroupBy)
            var theoNamHoc = await _context.DeTais
                .GroupBy(d => new { d.NamHoc, d.HocKy })
                .Select(g => new { g.Key.NamHoc, g.Key.HocKy, SoLuong = g.Count() })
                .OrderBy(x => x.NamHoc).ThenBy(x => x.HocKy)
                .ToListAsync();
            vm.DeTaiTheoNamHocHocKy = theoNamHoc
                .Select(x => new ThongKeItem { Nhan = $"{x.NamHoc} - {x.HocKy}", SoLuong = x.SoLuong })
                .ToList();

            return View(vm);
        }

        private static string NhanTrangThaiDangKy(string trangThai) => trangThai switch
        {
            "ChoDuyet" => "Chờ duyệt",
            "DaDuyet" => "Đã duyệt",
            "TuChoi" => "Từ chối",
            "DaHuy" => "Đã hủy",
            _ => trangThai
        };

        // ===== AJAX: dữ liệu biểu đồ trang Thống kê (JSON) =====
        [HttpGet]
        public async Task<IActionResult> DuLieuJson()
        {
            var deTaiTheoLinhVuc = await _context.DeTais
                .GroupBy(d => d.LinhVuc!.TenLinhVuc)
                .Select(g => new { nhan = g.Key, soLuong = g.Count() })
                .OrderByDescending(x => x.soLuong)
                .ToListAsync();

            var dkTheoTrangThai = (await _context.DangKyDeTais
                    .GroupBy(dk => dk.TrangThai)
                    .Select(g => new { TrangThai = g.Key, SoLuong = g.Count() })
                    .ToListAsync())
                .Select(x => new { nhan = NhanTrangThaiDangKy(x.TrangThai), soLuong = x.SoLuong })
                .OrderByDescending(x => x.soLuong)
                .ToList();

            var dkTheoThang = (await _context.DangKyDeTais
                    .GroupBy(dk => new { dk.NgayDangKy.Year, dk.NgayDangKy.Month })
                    .Select(g => new { g.Key.Year, g.Key.Month, SoLuong = g.Count() })
                    .OrderBy(x => x.Year).ThenBy(x => x.Month)
                    .ToListAsync())
                .Select(x => new { nhan = $"{x.Month:00}/{x.Year}", soLuong = x.SoLuong })
                .ToList();

            var deTaiTheoNamHoc = (await _context.DeTais
                    .GroupBy(d => new { d.NamHoc, d.HocKy })
                    .Select(g => new { g.Key.NamHoc, g.Key.HocKy, SoLuong = g.Count() })
                    .OrderBy(x => x.NamHoc).ThenBy(x => x.HocKy)
                    .ToListAsync())
                .Select(x => new { nhan = $"{x.NamHoc} - {x.HocKy}", soLuong = x.SoLuong })
                .ToList();

            var taiGiangVien = (await _context.GiangViens
                    .Select(g => new
                    {
                        g.HoTen,
                        SoToiDa = g.SoSinhVienHuongDanToiDa,
                        SoDangHuongDan = g.DanhSachPhanCong.Count(p => p.TrangThai == "DangHuongDan")
                    })
                    .OrderByDescending(x => x.SoDangHuongDan).ThenBy(x => x.HoTen)
                    .ToListAsync())
                .Select(x => new
                {
                    x.HoTen,
                    x.SoDangHuongDan,
                    x.SoToiDa,
                    TyLeSuDung = x.SoToiDa > 0 ? Math.Round(x.SoDangHuongDan * 100.0 / x.SoToiDa, 1) : 0
                })
                .ToList();

            var choCon = await _context.DeTais
                .Select(d => new
                {
                    d.TenDeTai,
                    SoDaDuyet = d.DanhSachDangKy.Count(dk => dk.TrangThai == "DaDuyet"),
                    d.SoSinhVienToiDa
                })
                .OrderBy(x => x.TenDeTai)
                .ToListAsync();

            return Json(new
            {
                linhVuc = deTaiTheoLinhVuc,
                trangThai = dkTheoTrangThai,
                thang = dkTheoThang,
                namHoc = deTaiTheoNamHoc,
                giangVien = taiGiangVien,
                choCon = choCon.Select(x => new
                {
                    x.TenDeTai,
                    x.SoDaDuyet,
                    SoChoCon = Math.Max(0, x.SoSinhVienToiDa - x.SoDaDuyet)
                })
            });
        }

        // ===== AJAX: dữ liệu Dashboard (JSON) =====
        [HttpGet]
        public async Task<IActionResult> DashboardJson()
        {
            var deTaiVaChoCon = _context.DeTais.Select(d => new
            {
                d.TrangThai,
                SoChoCon = d.SoSinhVienToiDa - d.DanhSachDangKy.Count(dk => dk.TrangThai == "DaDuyet")
            });

            return Json(new
            {
                soDeTaiConCho = await deTaiVaChoCon.CountAsync(x => x.TrangThai == "MoDangKy" && x.SoChoCon > 0),
                soDeTaiDuSoLuong = await deTaiVaChoCon.CountAsync(x => x.SoChoCon <= 0),
                soDangKyChoDuyet = await _context.DangKyDeTais.CountAsync(dk => dk.TrangThai == "ChoDuyet"),
                soDangKyDaDuyet = await _context.DangKyDeTais.CountAsync(dk => dk.TrangThai == "DaDuyet")
            });
        }

        [HttpGet]
        public async Task<IActionResult> XuatExcel()
        {
            var bang = await _xuat.LayCacBangThongKeAsync();
            return File(_xuat.XuatThongKeExcel(bang),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"ThongKe_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
        }

        [HttpGet]
        public async Task<IActionResult> XuatPdf()
        {
            var bang = await _xuat.LayCacBangThongKeAsync();
            return File(_xuat.XuatThongKePdf(bang), "application/pdf",
                $"ThongKe_{DateTime.Now:yyyyMMdd_HHmm}.pdf");
        }
    }
}