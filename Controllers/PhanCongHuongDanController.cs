// Họ và tên: Hoàng Kim
// Mã sinh viên: <23203100056>
// Nội dung thực hiện: Module 4 - Danh sách đăng ký chờ duyệt (tìm kiếm/lọc/phân trang),
// duyệt và từ chối đăng ký, phân công giảng viên hướng dẫn, thay đổi GVHD (giữ lịch sử).

using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyDeTaiDoAn.Data;
using QuanLyDeTaiDoAn.Filters;
using QuanLyDeTaiDoAn.Helpers;
using QuanLyDeTaiDoAn.Models;
using QuanLyDeTaiDoAn.Services;
using QuanLyDeTaiDoAn.ViewModels;

namespace QuanLyDeTaiDoAn.Controllers
{
    [YeuCauAdmin]
    public class PhanCongHuongDanController : Controller
    {
        private const int PageSize = 10;

        private readonly ApplicationDbContext _context;
        private readonly NghiepVuService _nghiepVu;

        private readonly XuatFileService _xuat;

        public PhanCongHuongDanController(ApplicationDbContext context, NghiepVuService nghiepVu, XuatFileService xuat)
        {
            _context = context;
            _nghiepVu = nghiepVu;
            _xuat = xuat;
        }

        // Quay lại đúng trang danh sách (giữ nguyên từ khóa, bộ lọc, số trang)
        private IActionResult QuayLai(string? quayLai)
        {
            if (!string.IsNullOrEmpty(quayLai) && Url.IsLocalUrl(quayLai))
                return LocalRedirect(quayLai);
            return RedirectToAction(nameof(DanhSachChoDuyet));
        }

        // ============================================================
        // DANH SÁCH ĐĂNG KÝ: Truy vấn -> Tìm kiếm -> Lọc -> Sắp xếp -> Phân trang
        // Mặc định hiển thị đăng ký "ChoDuyet"; trangThai = "TatCa" để xem mọi trạng thái.
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> DanhSachChoDuyet(
            string? tuKhoa, int? maLinhVuc, string trangThai = TrangThaiDangKy.ChoDuyet, int page = 1)
        {

            var query = _context.DangKyDeTais.AsQueryable();

            // Lọc theo trạng thái
            if (trangThai != "TatCa")
                query = query.Where(dk => dk.TrangThai == trangThai);

            // Lọc theo lĩnh vực của đề tài
            if (maLinhVuc.HasValue)
                query = query.Where(dk => dk.DeTai!.MaLinhVuc == maLinhVuc.Value);

            // Tìm theo mã SV / tên SV / tên đề tài
            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                tuKhoa = tuKhoa.Trim();
                bool laSo = int.TryParse(tuKhoa, out int maSv);
                query = query.Where(dk =>
                    dk.SinhVien!.HoTen.Contains(tuKhoa)
                    || dk.DeTai!.TenDeTai.Contains(tuKhoa)
                    || (laSo && dk.MaSinhVien == maSv));
            }

            // Phân trang
            int tongSo = await query.CountAsync();
            int tongTrang = Math.Max(1, (int)Math.Ceiling(tongSo / (double)PageSize));
            page = Math.Clamp(page, 1, tongTrang);

            var items = await query
                .OrderBy(dk => dk.NgayDangKy)                 // đăng ký cũ nhất xử lý trước
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .Select(dk => new DangKyDuyetItem
                {
                    MaDangKy = dk.MaDangKy,
                    MaSinhVien = dk.MaSinhVien,
                    TenSinhVien = dk.SinhVien!.HoTen,
                    MaDeTai = dk.MaDeTai,
                    TenDeTai = dk.DeTai!.TenDeTai,
                    TenLinhVuc = dk.DeTai.LinhVuc!.TenLinhVuc,
                    NgayDangKy = dk.NgayDangKy,
                    LyDoDangKy = dk.LyDoDangKy,
                    TrangThaiThucHien = dk.TrangThaiThucHien,
                    TrangThai = dk.TrangThai,
                    GhiChuDuyet = dk.GhiChuDuyet,
                    SoChoCon = dk.DeTai.SoSinhVienToiDa
                               - dk.DeTai.DanhSachDangKy.Count(x => x.TrangThai == TrangThaiDangKy.DaDuyet),
                    MaPhanCongHienTai = dk.DanhSachPhanCong
                        .Where(p => p.TrangThai == TrangThaiPhanCong.DangHuongDan)
                        .Select(p => (int?)p.MaPhanCong)
                        .FirstOrDefault(),
                    TenGiangVienHienTai = dk.DanhSachPhanCong
                        .Where(p => p.TrangThai == TrangThaiPhanCong.DangHuongDan)
                        .Select(p => p.GiangVien!.HoTen)
                        .FirstOrDefault()
                })
                .ToListAsync();

            var vm = new DanhSachChoDuyetViewModel
            {
                Items = items,
                TuKhoa = tuKhoa,
                MaLinhVuc = maLinhVuc,
                TrangThai = trangThai,
                Page = page,
                TotalPages = tongTrang,
                TotalItems = tongSo,
                LinhVucs = new SelectList(
                    await _context.LinhVucs.OrderBy(l => l.TenLinhVuc).ToListAsync(),
                    "MaLinhVuc", "TenLinhVuc", maLinhVuc)
            };

            return View(vm);
        }

        // CẬP NHẬT TIẾN ĐỘ (mục 8.6): chỉ tiến từng bước ChuaBatDau -> DangThucHien -> HoanThanh.
        // Khi HoanThanh: đóng phân công đang hiệu lực (KetThuc + NgayKetThuc) để trả suất hướng dẫn,
        // KHÔNG xóa gì nên lịch sử đăng ký/phân công được giữ nguyên.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTienDo(int id, string tienDo, string? quayLai)
        {
            if (!TienDoThucHien.ThuTu.Contains(tienDo))
            {
                TempData["Loi"] = "Trạng thái thực hiện không hợp lệ.";
                return QuayLai(quayLai);
            }

            await using var tx = await _context.Database.BeginTransactionAsync();

            var dk = await _context.DangKyDeTais
                .Include(x => x.DanhSachPhanCong)
                .FirstOrDefaultAsync(x => x.MaDangKy == id);
            if (dk == null) return NotFound();

            if (dk.TrangThai != TrangThaiDangKy.DaDuyet)
            {
                TempData["Loi"] = "Chỉ đăng ký đã duyệt mới cập nhật được tiến độ.";
                return QuayLai(quayLai);
            }

            int hienTai = Array.IndexOf(TienDoThucHien.ThuTu, dk.TrangThaiThucHien);
            int moi = Array.IndexOf(TienDoThucHien.ThuTu, tienDo);
            if (moi != hienTai + 1)
            {
                TempData["Loi"] = "Chỉ được chuyển tiến độ sang bước kế tiếp.";
                return QuayLai(quayLai);
            }

            var pc = dk.DanhSachPhanCong.FirstOrDefault(p => p.TrangThai == TrangThaiPhanCong.DangHuongDan);
            if (pc == null)
            {
                TempData["Loi"] = "Cần phân công giảng viên hướng dẫn trước khi cập nhật tiến độ.";
                return QuayLai(quayLai);
            }

            if (tienDo == TienDoThucHien.HoanThanh)
            {
                pc.TrangThai = TrangThaiPhanCong.KetThuc;
                pc.NgayKetThuc = DateTime.Now;
            }

            dk.TrangThaiThucHien = tienDo;
            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            TempData["ThanhCong"] = $"Đã cập nhật tiến độ đăng ký #{id}.";
            return QuayLai(quayLai);
        }

        // ============================================================
        // DUYỆT ĐĂNG KÝ (POST)
        // Kiểm tra lại toàn bộ điều kiện tại thời điểm duyệt, trong transaction Serializable
        // để hai lần duyệt đồng thời không làm đề tài vượt số chỗ.
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Duyet(int id, string? quayLai)
        {

            await using var tx = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            var kq = await _nghiepVu.KiemTraDieuKienDuyetAsync(id);
            if (!kq.HopLe)
            {
                TempData["Loi"] = string.Join("|", kq.Loi);   // tx tự rollback khi dispose
                return QuayLai(quayLai);
            }

            var dk = await _context.DangKyDeTais.FindAsync(id);
            if (dk == null) return NotFound();

            dk.TrangThai = TrangThaiDangKy.DaDuyet;
            dk.GhiChuDuyet = "Đủ điều kiện, đã được duyệt";
            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            TempData["ThanhCong"] = $"Đã duyệt đăng ký #{id}. Bạn có thể phân công giảng viên hướng dẫn.";
            return QuayLai(quayLai);
        }

        // ============================================================
        // TỪ CHỐI ĐĂNG KÝ (POST) - bắt buộc nhập lý do, lưu vào GhiChuDuyet
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TuChoi(int id, string? ghiChu, string? quayLai)
        {

            if (string.IsNullOrWhiteSpace(ghiChu))
            {
                TempData["Loi"] = "Vui lòng nhập lý do từ chối.";
                return QuayLai(quayLai);
            }

            var dk = await _context.DangKyDeTais.FindAsync(id);
            if (dk == null) return NotFound();

            if (dk.TrangThai != TrangThaiDangKy.ChoDuyet)
            {
                TempData["Loi"] = $"Chỉ đăng ký ở trạng thái chờ duyệt mới được từ chối (hiện tại: {dk.TrangThai}).";
                return QuayLai(quayLai);
            }

            dk.TrangThai = TrangThaiDangKy.TuChoi;
            dk.GhiChuDuyet = ghiChu.Trim();
            await _context.SaveChangesAsync();

            TempData["ThanhCong"] = $"Đã từ chối đăng ký #{id}.";
            return QuayLai(quayLai);
        }

        // ===== AJAX: Duyệt đăng ký, trả JSON =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DuyetAjax(int id)
        {
            await using var tx = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            var kq = await _nghiepVu.KiemTraDieuKienDuyetAsync(id);
            if (!kq.HopLe)
                return Json(new { thanhCong = false, loi = kq.Loi });

            var dk = await _context.DangKyDeTais.FindAsync(id);
            if (dk == null) return Json(new { thanhCong = false, loi = new[] { "Không tìm thấy đăng ký." } });

            dk.TrangThai = TrangThaiDangKy.DaDuyet;
            dk.GhiChuDuyet = "Đủ điều kiện, đã được duyệt";
            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            // Số chỗ còn mới của đề tài (tính lại bằng LINQ) để cập nhật các dòng cùng đề tài
            int? soChoCon = await _nghiepVu.TinhSoChoConAsync(dk.MaDeTai);

            return Json(new
            {
                thanhCong = true,
                thongBao = $"Đã duyệt đăng ký #{id}.",
                maDeTai = dk.MaDeTai,
                soChoCon
            });
        }

        // ===== AJAX: Từ chối đăng ký, trả JSON =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TuChoiAjax(int id, string? ghiChu)
        {
            if (string.IsNullOrWhiteSpace(ghiChu))
                return Json(new { thanhCong = false, loi = new[] { "Vui lòng nhập lý do từ chối." } });

            var dk = await _context.DangKyDeTais.FindAsync(id);
            if (dk == null)
                return Json(new { thanhCong = false, loi = new[] { "Không tìm thấy đăng ký." } });

            if (dk.TrangThai != TrangThaiDangKy.ChoDuyet)
                return Json(new { thanhCong = false, loi = new[] { $"Chỉ đăng ký chờ duyệt mới được từ chối (hiện tại: {dk.TrangThai})." } });

            dk.TrangThai = TrangThaiDangKy.TuChoi;
            dk.GhiChuDuyet = ghiChu.Trim();
            await _context.SaveChangesAsync();

            return Json(new { thanhCong = true, thongBao = $"Đã từ chối đăng ký #{id}." });
        }

        // ============================================================
        // Nạp thông tin hiển thị + danh sách giảng viên phù hợp cho form.
        // Gọi lại mỗi khi trả View (kể cả khi Validation lỗi) vì dropdown không được post lại.
        // Trả false nếu không tìm thấy đăng ký.
        // ============================================================
        private async Task<bool> NapThongTinAsync(PhanCongViewModel vm)
        {
            var info = await _context.DangKyDeTais
                .Where(dk => dk.MaDangKy == vm.MaDangKy)
                .Select(dk => new
                {
                    dk.SinhVien!.HoTen,
                    dk.DeTai!.TenDeTai,
                    TenLinhVuc = dk.DeTai.LinhVuc!.TenLinhVuc
                })
                .FirstOrDefaultAsync();
            if (info == null) return false;

            vm.TenSinhVien = info.HoTen;
            vm.TenDeTai = info.TenDeTai;
            vm.TenLinhVuc = info.TenLinhVuc;

            var pcHienTai = await _context.PhanCongHuongDans
                .Where(p => p.MaDangKy == vm.MaDangKy && p.TrangThai == TrangThaiPhanCong.DangHuongDan)
                .Select(p => new { p.MaGiangVien, p.GiangVien!.HoTen, p.NgayBatDau })
                .FirstOrDefaultAsync();

            vm.TenGiangVienHienTai = pcHienTai?.HoTen;
            vm.NgayBatDauHienTai = pcHienTai?.NgayBatDau;

            // Khi đổi GVHD thì loại giảng viên hiện tại khỏi danh sách chọn
            int? loaiTru = vm.LaThayDoi ? pcHienTai?.MaGiangVien : null;
            vm.GiangVienPhuHop = await _nghiepVu.LayGiangVienPhuHopAsync(vm.MaDangKy, loaiTru);
            return true;
        }

        // ============================================================
        // PHÂN CÔNG GVHD - GET: hiển thị form
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> PhanCong(int maDangKy)
        {

            var dk = await _context.DangKyDeTais
                .Where(x => x.MaDangKy == maDangKy)
                .Select(x => new
                {
                    x.TrangThai,
                    MaPhanCong = x.DanhSachPhanCong
                        .Where(p => p.TrangThai == TrangThaiPhanCong.DangHuongDan)
                        .Select(p => (int?)p.MaPhanCong)
                        .FirstOrDefault(),
                    x.TrangThaiThucHien
                })
                .FirstOrDefaultAsync();
            if (dk == null) return NotFound();

            if (dk.TrangThai != TrangThaiDangKy.DaDuyet)
            {
                TempData["Loi"] = "Chỉ đăng ký đã duyệt mới được phân công giảng viên.";
                return RedirectToAction(nameof(DanhSachChoDuyet), new { trangThai = TrangThaiDangKy.DaDuyet });
            }
            if (dk.MaPhanCong != null)
            {
                TempData["Loi"] = "Đăng ký này đã có giảng viên hướng dẫn. Hãy dùng chức năng Thay đổi GVHD.";
                return RedirectToAction(nameof(ThayDoiGVHD), new { maPhanCong = dk.MaPhanCong });
            }

            var vm = new PhanCongViewModel { MaDangKy = maDangKy };
            await NapThongTinAsync(vm);
            return View(vm);
        }

        // ============================================================
        // PHÂN CÔNG GVHD - POST: tạo bản ghi PhanCongHuongDan mới
        // Kiểm tra lại toàn bộ điều kiện trong transaction Serializable.
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PhanCong(PhanCongViewModel vm)
        {

            if (!await NapThongTinAsync(vm)) return NotFound();

            if (!ModelState.IsValid) return View(vm);   // dropdown đã nạp lại ở trên

            await using var tx = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            var kq = await _nghiepVu.KiemTraDieuKienPhanCongAsync(vm.MaDangKy, vm.MaGiangVien!.Value);
            if (!kq.HopLe)
            {
                foreach (var loi in kq.Loi) ModelState.AddModelError(string.Empty, loi);
                return View(vm);   // transaction tự rollback khi dispose
            }

            _context.PhanCongHuongDans.Add(new PhanCongHuongDan
            {
                MaDangKy = vm.MaDangKy,
                MaGiangVien = vm.MaGiangVien.Value,
                NgayBatDau = DateTime.Now,
                NgayKetThuc = null,                       // đang hiệu lực -> để trống
                TrangThai = TrangThaiPhanCong.DangHuongDan,
                GhiChu = string.IsNullOrWhiteSpace(vm.GhiChu) ? "Phân công lần đầu" : vm.GhiChu.Trim()
            });
            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            TempData["ThanhCong"] = $"Đã phân công giảng viên hướng dẫn cho đăng ký #{vm.MaDangKy}.";
            return RedirectToAction(nameof(DanhSachChoDuyet), new { trangThai = TrangThaiDangKy.DaDuyet });
        }

        // ============================================================
        // THAY ĐỔI GVHD - GET: hiển thị GV hiện tại + dropdown GV mới
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> ThayDoiGVHD(int maPhanCong)
        {
            var pc = await _context.PhanCongHuongDans.FindAsync(maPhanCong);
            if (pc == null) return NotFound();

            if (pc.TrangThai != TrangThaiPhanCong.DangHuongDan)
            {
                TempData["Loi"] = "Phân công này không còn hiệu lực nên không thể thay đổi.";
                return RedirectToAction(nameof(DanhSachChoDuyet), new { trangThai = TrangThaiDangKy.DaDuyet });
            }

            var vm = new PhanCongViewModel { MaDangKy = pc.MaDangKy, MaPhanCongCu = pc.MaPhanCong };
            await NapThongTinAsync(vm);
            return View(vm);
        }

        // ============================================================
        // THAY ĐỔI GVHD - POST (mục 8.5)
        // KHÔNG sửa MaGiangVien của bản ghi cũ. Thay vào đó, trong CÙNG một transaction:
        //   1) bản ghi cũ  -> "DaThayDoi" + NgayKetThuc
        //   2) bản ghi mới -> GV mới, "DangHuongDan", NgayBatDau
        // Cả hai cùng thành công hoặc cùng rollback => lịch sử luôn toàn vẹn.
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThayDoiGVHD(PhanCongViewModel vm)
        {
            if (vm.MaPhanCongCu == null) return BadRequest();
            if (!await NapThongTinAsync(vm)) return NotFound();

            if (string.IsNullOrWhiteSpace(vm.GhiChu))
                ModelState.AddModelError(nameof(vm.GhiChu), "Vui lòng nhập lý do thay đổi giảng viên.");

            if (!ModelState.IsValid) return View(vm);

            await using var tx = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            // GV mới phải qua đủ các kiểm tra như phân công lần đầu (lĩnh vực, tải, đang công tác...)
            var kq = await _nghiepVu.KiemTraDieuKienPhanCongAsync(
                vm.MaDangKy, vm.MaGiangVien!.Value, vm.MaPhanCongCu);
            if (!kq.HopLe)
            {
                foreach (var loi in kq.Loi) ModelState.AddModelError(string.Empty, loi);
                return View(vm);
            }

            var pcCu = await _context.PhanCongHuongDans.FindAsync(vm.MaPhanCongCu.Value);
            if (pcCu == null) return NotFound();

            var bayGio = DateTime.Now;

            // (1) Đóng bản ghi cũ - giữ nguyên MaGiangVien cũ để truy vết
            pcCu.TrangThai = TrangThaiPhanCong.DaThayDoi;
            pcCu.NgayKetThuc = bayGio;

            // (2) Tạo bản ghi mới cho giảng viên mới
            _context.PhanCongHuongDans.Add(new PhanCongHuongDan
            {
                MaDangKy = pcCu.MaDangKy,
                MaGiangVien = vm.MaGiangVien.Value,
                NgayBatDau = bayGio,
                NgayKetThuc = null,
                TrangThai = TrangThaiPhanCong.DangHuongDan,
                GhiChu = vm.GhiChu!.Trim()
            });

            await _context.SaveChangesAsync();   // một SaveChanges + transaction => nguyên tử
            await tx.CommitAsync();

            TempData["ThanhCong"] = $"Đã thay đổi giảng viên hướng dẫn cho đăng ký #{pcCu.MaDangKy}. Lịch sử phân công được giữ nguyên.";
            return RedirectToAction(nameof(DanhSachChoDuyet), new { trangThai = TrangThaiDangKy.DaDuyet });
        }

        // ============================================================
        // LỊCH SỬ PHÂN CÔNG của một đăng ký (mục 8.9): ai hướng dẫn, từ khi nào, đến khi nào
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> LichSuPhanCong(int maDangKy)
        {

            var vm = await _context.DangKyDeTais
                .Where(dk => dk.MaDangKy == maDangKy)
                .Select(dk => new LichSuPhanCongViewModel
                {
                    MaDangKy = dk.MaDangKy,
                    TenSinhVien = dk.SinhVien!.HoTen,
                    TenDeTai = dk.DeTai!.TenDeTai,
                    TenLinhVuc = dk.DeTai.LinhVuc!.TenLinhVuc,
                    TrangThaiDangKy = dk.TrangThai
                })
                .FirstOrDefaultAsync();
            if (vm == null) return NotFound();

            vm.DanhSach = await _context.PhanCongHuongDans
                .Where(p => p.MaDangKy == maDangKy)
                .OrderBy(p => p.NgayBatDau)          // cũ -> mới
                .ThenBy(p => p.MaPhanCong)
                .Select(p => new LichSuPhanCongItem
                {
                    MaPhanCong = p.MaPhanCong,
                    TenGiangVien = p.GiangVien!.HoTen,
                    HocVi = p.GiangVien.HocVi,
                    NgayBatDau = p.NgayBatDau,
                    NgayKetThuc = p.NgayKetThuc,
                    TrangThai = p.TrangThai,
                    GhiChu = p.GhiChu
                })
                .ToListAsync();

            return View(vm);
        }
        // ===== AJAX: danh sách GV phù hợp (đúng lĩnh vực, đang công tác, còn tải) =====
        [HttpGet]
        public async Task<IActionResult> LayGiangVienJson(int maDangKy, int? maPhanCongCu)
        {
            int? loaiTru = null;
            if (maPhanCongCu.HasValue)
            {
                loaiTru = await _context.PhanCongHuongDans
                    .Where(p => p.MaPhanCong == maPhanCongCu.Value && p.TrangThai == TrangThaiPhanCong.DangHuongDan)
                    .Select(p => (int?)p.MaGiangVien)
                    .FirstOrDefaultAsync();
            }

            var ds = await _nghiepVu.LayGiangVienPhuHopAsync(maDangKy, loaiTru);
            return Json(ds.Select(g => new
            {
                g.MaGiangVien,
                g.HoTen,
                g.TenLinhVuc,
                g.SoDangHuongDan,
                g.SoToiDa,
                SoSuatCon = g.SoToiDa - g.SoDangHuongDan
            }));
        }

        // ===== AJAX: chi tiết 1 giảng viên (hiện khi chọn trong dropdown) =====
        [HttpGet]
        public async Task<IActionResult> ChiTietGiangVienJson(int maGiangVien)
        {
            var gv = await _context.GiangViens
                .Where(g => g.MaGiangVien == maGiangVien)
                .Select(g => new
                {
                    g.HoTen,
                    g.HocVi,
                    g.Email,
                    TenLinhVuc = g.LinhVuc!.TenLinhVuc,
                    g.SoSinhVienHuongDanToiDa,
                    SoDangHuongDan = g.DanhSachPhanCong.Count(p => p.TrangThai == TrangThaiPhanCong.DangHuongDan),
                    TongPhanCong = g.DanhSachPhanCong.Count()
                })
                .FirstOrDefaultAsync();

            return gv == null ? NotFound() : Json(gv);
        }

        // Xuất đúng danh sách đang lọc (cùng tham số với DanhSachChoDuyet, bỏ phân trang)
        [HttpGet]
        public async Task<IActionResult> XuatExcel(string? tuKhoa, int? maLinhVuc,
            string trangThai = TrangThaiDangKy.ChoDuyet)
        {
            var ds = await _xuat.LayDanhSachDangKyAsync(tuKhoa, maLinhVuc, trangThai);
            var bytes = _xuat.XuatDangKyExcel(ds);
            return File(bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"DanhSachDangKy_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
        }

        [HttpGet]
        public async Task<IActionResult> XuatPdf(string? tuKhoa, int? maLinhVuc,
            string trangThai = TrangThaiDangKy.ChoDuyet)
        {
            var ds = await _xuat.LayDanhSachDangKyAsync(tuKhoa, maLinhVuc, trangThai);
            string moTa = $"Trạng thái: {(trangThai == "TatCa" ? "Tất cả" : trangThai)}"
                        + (string.IsNullOrWhiteSpace(tuKhoa) ? "" : $" • Từ khóa: {tuKhoa}");
            var bytes = _xuat.XuatDangKyPdf(ds, moTa);
            return File(bytes, "application/pdf",
                $"DanhSachDangKy_{DateTime.Now:yyyyMMdd_HHmm}.pdf");
        }
    }
}
