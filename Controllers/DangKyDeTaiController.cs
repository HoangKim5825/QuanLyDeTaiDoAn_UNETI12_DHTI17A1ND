// Họ và tên: <ĐIỀN TÊN>
// Mã sinh viên: <ĐIỀN MÃ SV>
// Nội dung thực hiện: Module 3 - Đăng ký đề tài, kiểm tra điều kiện đăng ký bằng LINQ,
// hủy đăng ký chờ duyệt, theo dõi kết quả đăng ký của chính sinh viên
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDeTaiDoAn.Data;
using QuanLyDeTaiDoAn.Filters;
using QuanLyDeTaiDoAn.Helpers;
using QuanLyDeTaiDoAn.Models;
using QuanLyDeTaiDoAn.Services;
using QuanLyDeTaiDoAn.ViewModels;

namespace QuanLyDeTaiDoAn.Controllers
{
    [YeuCauDangNhap]
    public class DangKyDeTaiController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly NghiepVuService _nghiepVu;

        public DangKyDeTaiController(ApplicationDbContext context, NghiepVuService nghiepVu)
        {
            _context = context;
            _nghiepVu = nghiepVu;
        }

        // Vai trò lấy từ Session do server ghi, KHÔNG tin dữ liệu từ View
        private bool LaSinhVien =>
            HttpContext.Session.GetString(YeuCauDangNhapAttribute.SessionVaiTro) == VaiTroTaiKhoan.SinhVien;

        // MaSinhVien xác định từ tài khoản đăng nhập, không nhận từ URL/form
        private async Task<int?> LayMaSinhVienAsync()
        {
            int? maTk = HttpContext.Session.GetInt32(YeuCauDangNhapAttribute.SessionMaTaiKhoan);
            if (maTk == null) return null;
            return await _context.SinhViens
                .Where(s => s.MaTaiKhoan == maTk)
                .Select(s => (int?)s.MaSinhVien)
                .FirstOrDefaultAsync();
        }

        // =====================================================================
        // THEO DÕI KẾT QUẢ: danh sách đăng ký của chính sinh viên
        // =====================================================================
        [HttpGet]
        public async Task<IActionResult> Index(string? trangThai)
        {
            // Admin quản lý đăng ký ở Module 4
            if (!LaSinhVien)
                return RedirectToAction("DanhSachChoDuyet", "PhanCongHuongDan", new { trangThai = "TatCa" });

            int? maSv = await LayMaSinhVienAsync();
            ViewBag.TrangThai = trangThai;
            if (maSv == null)
            {
                ViewBag.ChuaCoHoSo = true;
                return View(new List<DangKyCuaToiItem>());
            }

            var query = _context.DangKyDeTais.AsNoTracking().Where(dk => dk.MaSinhVien == maSv);
            if (!string.IsNullOrEmpty(trangThai))
                query = query.Where(dk => dk.TrangThai == trangThai);

            var ds = await query
                .OrderByDescending(dk => dk.NgayDangKy)
                .Select(dk => new DangKyCuaToiItem
                {
                    MaDangKy = dk.MaDangKy,
                    MaDeTai = dk.MaDeTai,
                    TenDeTai = dk.DeTai!.TenDeTai,
                    TenLinhVuc = dk.DeTai.LinhVuc!.TenLinhVuc,
                    NamHoc = dk.DeTai.NamHoc,
                    HocKy = dk.DeTai.HocKy,
                    NgayDangKy = dk.NgayDangKy,
                    TrangThai = dk.TrangThai,
                    TrangThaiThucHien = dk.TrangThaiThucHien,
                    GhiChuDuyet = dk.GhiChuDuyet,
                    TenGiangVienHienTai = dk.DanhSachPhanCong
                        .Where(p => p.TrangThai == TrangThaiPhanCong.DangHuongDan)
                        .Select(p => p.GiangVien!.HoTen)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return View(ds);
        }

        // =====================================================================
        // CHI TIẾT: chỉ xem được đăng ký của CHÍNH MÌNH (đổi ID trên URL -> 404)
        // =====================================================================
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (!LaSinhVien) return RedirectToAction("AccessDenied", "Account");
            int? maSv = await LayMaSinhVienAsync();
            if (maSv == null) return NotFound();

            var vm = await _context.DangKyDeTais.AsNoTracking()
                .Where(dk => dk.MaDangKy == id && dk.MaSinhVien == maSv)   // điều kiện sở hữu
                .Select(dk => new DangKyChiTietViewModel
                {
                    MaDangKy = dk.MaDangKy,
                    MaDeTai = dk.MaDeTai,
                    TenSinhVien = dk.SinhVien!.HoTen,
                    TenDeTai = dk.DeTai!.TenDeTai,
                    TenLinhVuc = dk.DeTai.LinhVuc!.TenLinhVuc,
                    NamHoc = dk.DeTai.NamHoc,
                    HocKy = dk.DeTai.HocKy,
                    NgayDangKy = dk.NgayDangKy,
                    LyDoDangKy = dk.LyDoDangKy,
                    TrangThai = dk.TrangThai,
                    TrangThaiThucHien = dk.TrangThaiThucHien,
                    GhiChuDuyet = dk.GhiChuDuyet,
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
                .FirstOrDefaultAsync();
            if (vm == null) return NotFound();

            vm.TenGiangVienHienTai = vm.LichSu
                .FirstOrDefault(p => p.TrangThai == TrangThaiPhanCong.DangHuongDan)?.TenGiangVien;
            return View(vm);
        }

        // =====================================================================
        // ĐĂNG KÝ - GET: hiển thị đề tài + kết quả kiểm tra điều kiện
        // (action và tên tham số "maDeTai" khớp link ở DeTai/Details)
        // =====================================================================
        [HttpGet]
        public async Task<IActionResult> Create(int maDeTai)
        {
            if (!LaSinhVien) return RedirectToAction("AccessDenied", "Account");
            int? maSv = await LayMaSinhVienAsync();
            if (maSv == null)
            {
                TempData["Loi"] = "Tài khoản của bạn chưa có hồ sơ sinh viên. Vui lòng liên hệ quản trị viên.";
                return RedirectToAction("Index", "DeTai");
            }

            var vm = new DangKyDeTaiCreateViewModel { MaDeTai = maDeTai };
            if (!await NapThongTinAsync(vm, maSv.Value)) return NotFound();

            vm.Loi = (await KiemTraDieuKienAsync(maSv.Value, maDeTai)).Loi;
            return View(vm);
        }

        // =====================================================================
        // ĐĂNG KÝ - POST: kiểm tra lại toàn bộ trong transaction Serializable
        // rồi mới lưu. NgayDangKy do hệ thống xác định.
        // =====================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaDeTai,LyDoDangKy")] DangKyDeTaiCreateViewModel input)
        {
            if (!LaSinhVien) return RedirectToAction("AccessDenied", "Account");
            int? maSv = await LayMaSinhVienAsync();
            if (maSv == null) return Forbid();

            if (!await NapThongTinAsync(input, maSv.Value)) return NotFound();   // nạp lại dữ liệu hiển thị

            if (!ModelState.IsValid) return View(input);

            await using var tx = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            var kq = await KiemTraDieuKienAsync(maSv.Value, input.MaDeTai);
            if (!kq.HopLe)
            {
                input.Loi = kq.Loi;
                await NapThongTinAsync(input, maSv.Value);   // cập nhật lại số chỗ còn
                return View(input);                          // tx tự rollback khi dispose
            }

            _context.DangKyDeTais.Add(new DangKyDeTai
            {
                MaSinhVien = maSv.Value,
                MaDeTai = input.MaDeTai,
                NgayDangKy = DateTime.Now,                    // hệ thống xác định
                LyDoDangKy = input.LyDoDangKy!.Trim(),
                TrangThai = TrangThaiDangKy.ChoDuyet,
                TrangThaiThucHien = TienDoThucHien.ChuaBatDau
            });
            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            TempData["ThongBao"] = $"Đã gửi đăng ký đề tài \"{input.TenDeTai}\". Vui lòng chờ quản trị viên duyệt.";
            return RedirectToAction(nameof(Index));
        }

        // =====================================================================
        // HỦY: chỉ đăng ký CỦA MÌNH và còn "ChoDuyet". Không xóa vật lý, chỉ đổi trạng thái.
        // =====================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Huy(int id)
        {
            if (!LaSinhVien) return RedirectToAction("AccessDenied", "Account");
            int? maSv = await LayMaSinhVienAsync();
            if (maSv == null) return Forbid();

            var dk = await _context.DangKyDeTais
                .FirstOrDefaultAsync(x => x.MaDangKy == id && x.MaSinhVien == maSv);
            if (dk == null) return NotFound();

            if (dk.TrangThai != TrangThaiDangKy.ChoDuyet)
            {
                TempData["Loi"] = "Chỉ được hủy đăng ký đang ở trạng thái Chờ duyệt. " +
                                  "Đăng ký đã được xử lý cần liên hệ quản trị viên.";
                return RedirectToAction(nameof(Index));
            }

            dk.TrangThai = TrangThaiDangKy.DaHuy;
            await _context.SaveChangesAsync();

            TempData["ThongBao"] = $"Đã hủy đăng ký #{id}.";
            return RedirectToAction(nameof(Index));
        }

        // =====================================================================
        // Hàm hỗ trợ
        // =====================================================================

        // Nạp thông tin hiển thị (đề tài, số chỗ còn, tên sinh viên). false nếu không có đề tài.
        private async Task<bool> NapThongTinAsync(DangKyDeTaiCreateViewModel vm, int maSv)
        {
            var dt = await _context.DeTais.AsNoTracking()
                .Where(d => d.MaDeTai == vm.MaDeTai)
                .Select(d => new
                {
                    d.TenDeTai,
                    TenLinhVuc = d.LinhVuc!.TenLinhVuc,
                    d.MoTa,
                    d.YeuCau,
                    d.NamHoc,
                    d.HocKy,
                    d.SoSinhVienToiDa
                })
                .FirstOrDefaultAsync();
            if (dt == null) return false;

            vm.TenDeTai = dt.TenDeTai;
            vm.TenLinhVuc = dt.TenLinhVuc;
            vm.MoTa = dt.MoTa;
            vm.YeuCau = dt.YeuCau;
            vm.NamHoc = dt.NamHoc;
            vm.HocKy = dt.HocKy;
            vm.SoSinhVienToiDa = dt.SoSinhVienToiDa;
            vm.SoChoCon = await _nghiepVu.TinhSoChoConAsync(vm.MaDeTai) ?? 0;   // dùng chung công thức với M4
            vm.TenSinhVien = await _context.SinhViens
                .Where(s => s.MaSinhVien == maSv).Select(s => s.HoTen).FirstOrDefaultAsync() ?? string.Empty;
            return true;
        }

        // KIỂM TRA ĐIỀU KIỆN ĐĂNG KÝ (mục 7.3) bằng EF Core + LINQ. Gom TẤT CẢ lỗi.
        private async Task<KetQuaKiemTra> KiemTraDieuKienAsync(int maSv, int maDeTai)
        {
            var kq = new KetQuaKiemTra();

            // (1) Sinh viên tồn tại và đang hoạt động
            var sv = await _context.SinhViens.AsNoTracking()
                .Where(s => s.MaSinhVien == maSv)
                .Select(s => new { s.TrangThai })
                .FirstOrDefaultAsync();
            if (sv == null) { kq.Them("Không tìm thấy hồ sơ sinh viên."); return kq; }
            if (!sv.TrangThai) kq.Them("Hồ sơ sinh viên không còn ở trạng thái hoạt động.");

            // (2) Đề tài tồn tại, đang Mở đăng ký, đúng đợt đang áp dụng
            var dt = await _context.DeTais.AsNoTracking().FirstOrDefaultAsync(d => d.MaDeTai == maDeTai);
            if (dt == null) { kq.Them("Đề tài không tồn tại."); return kq; }

            if (dt.TrangThai != TrangThaiDeTai.MoDangKy)
                kq.Them("Đề tài hiện không ở trạng thái Mở đăng ký.");

            if (dt.NamHoc != DotDangKy.NamHocApDung
                || (DotDangKy.HocKyApDung != null && dt.HocKy != DotDangKy.HocKyApDung))
                kq.Them($"Đề tài thuộc năm học {dt.NamHoc} - {dt.HocKy}, không nằm trong đợt đăng ký hiện tại.");

            // (3) Đề tài còn khả năng tiếp nhận: SoChoCon = SoSinhVienToiDa - số DaDuyet
            var soCho = await _nghiepVu.TinhSoChoConAsync(maDeTai);
            if (soCho == null || soCho <= 0)
                kq.Them("Đề tài đã đủ số lượng sinh viên.");

            // (4) Không có đăng ký DaDuyet khác trong cùng đợt (cùng năm học + học kỳ)
            int nam = dt.NamHoc;
            string hk = dt.HocKy;
            bool daCoDeTaiKhac = await _context.DangKyDeTais.AnyAsync(x =>
                x.MaSinhVien == maSv
                && x.TrangThai == TrangThaiDangKy.DaDuyet
                && x.DeTai!.NamHoc == nam
                && x.DeTai.HocKy == hk);
            if (daCoDeTaiKhac)
                kq.Them("Bạn đã có một đề tài khác đang hiệu lực trong cùng đợt.");

            // (5) Không tạo nhiều đăng ký Chờ duyệt (hoặc đã duyệt) cho cùng một đề tài
            bool daDangKyDeTaiNay = await _context.DangKyDeTais.AnyAsync(x =>
                x.MaSinhVien == maSv
                && x.MaDeTai == maDeTai
                && (x.TrangThai == TrangThaiDangKy.ChoDuyet || x.TrangThai == TrangThaiDangKy.DaDuyet));
            if (daDangKyDeTaiNay)
                kq.Them("Bạn đã có đăng ký đang chờ duyệt hoặc đã được duyệt cho đề tài này.");

            return kq;
        }
    }
}
