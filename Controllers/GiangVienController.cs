// Họ và tên: <ĐIỀN TÊN>
// Mã sinh viên: <ĐIỀN MÃ SV>
// Nội dung thực hiện: Quản lý giảng viên - CRUD, tìm kiếm, lọc, tải hướng dẫn (Module 2)
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyDeTaiDoAn.Constants;
using QuanLyDeTaiDoAn.Filters;   // nếu M1 đặt attribute ở namespace khác thì sửa dòng này
using QuanLyDeTaiDoAn.Data;
using QuanLyDeTaiDoAn.Extensions;
using QuanLyDeTaiDoAn.Models;
using QuanLyDeTaiDoAn.ViewModels;

namespace QuanLyDeTaiDoAn.Controllers
{
    [YeuCauAdmin]
    public class GiangVienController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GiangVienController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===== INDEX: tìm kiếm + lọc =====
        public async Task<IActionResult> Index(GiangVienFilterViewModel vm)
        {
            var items = LocGiangVien(vm);

            vm.KetQua = await items.OrderBy(x => x.HoTen).ThenBy(x => x.MaGiangVien).ToListAsync();

            vm.LinhVucs = await _context.LinhVucs.AsNoTracking()
                .OrderBy(l => l.TenLinhVuc)
                .Select(l => new SelectListItem(l.TenLinhVuc, l.MaLinhVuc.ToString()))
                .ToListAsync();
            vm.TrangThais = TrangThaiConst.GiangVienTT.TatCa
                .Select(t => new SelectListItem(TrangThaiConst.GiangVienTT.Nhan(t), t)).ToList();

            return View(vm);
        }

        // ===== DETAILS =====
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var gv = await _context.GiangViens.AsNoTracking()
                .Include(g => g.LinhVuc)
                .FirstOrDefaultAsync(g => g.MaGiangVien == id);
            if (gv == null) return NotFound();

            var so = await _context.GiangViens.Where(g => g.MaGiangVien == id)
                .ToListItems().FirstAsync();

            return View(new GiangVienDetailsViewModel
            {
                GiangVien = gv,
                TaiHienTai = so.TaiHienTai,
                TongPhanCong = so.TongPhanCong
            });
        }

        // ===== CREATE =====
        public async Task<IActionResult> Create()
        {
            var gv = new GiangVien
            {
                SoSinhVienHuongDanToiDa = 3,
                TrangThai = TrangThaiConst.GiangVienTT.DangCongTac,
                MaLinhVuc = await _context.LinhVucs.Where(l => l.TrangThai)
                    .OrderBy(l => l.TenLinhVuc).Select(l => l.MaLinhVuc).FirstOrDefaultAsync()
            };
            await NapDropdownFormAsync(gv.MaLinhVuc);
            return View(gv);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("HoTen,HocVi,Email,SoDienThoai,MaLinhVuc,SoSinhVienHuongDanToiDa,TrangThai")] GiangVien gv)
        {
            await KiemTraNghiepVuAsync(gv, null);

            if (!ModelState.IsValid)
            {
                await NapDropdownFormAsync(gv.MaLinhVuc);
                return View(gv);
            }

            _context.GiangViens.Add(gv);
            await _context.SaveChangesAsync();
            TempData["ThongBao"] = $"Đã thêm giảng viên \"{gv.HoTen}\".";
            return RedirectToAction(nameof(Index));
        }

        // ===== EDIT =====
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var gv = await _context.GiangViens.AsNoTracking().FirstOrDefaultAsync(g => g.MaGiangVien == id);
            if (gv == null) return NotFound();

            ViewBag.TaiHienTai = await DemTaiHienTaiAsync(gv.MaGiangVien);
            await NapDropdownFormAsync(gv.MaLinhVuc);
            return View(gv);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("MaGiangVien,HoTen,HocVi,Email,SoDienThoai,MaLinhVuc,SoSinhVienHuongDanToiDa,TrangThai")] GiangVien input)
        {
            if (id != input.MaGiangVien) return NotFound();

            var gv = await _context.GiangViens.FindAsync(id);
            if (gv == null) return NotFound();

            await KiemTraNghiepVuAsync(input, id);

            // Không cho giảm giới hạn thấp hơn số sinh viên đang hướng dẫn
            int tai = await DemTaiHienTaiAsync(id);
            if (input.SoSinhVienHuongDanToiDa < tai)
                ModelState.AddModelError(nameof(input.SoSinhVienHuongDanToiDa),
                    $"Giảng viên đang hướng dẫn {tai} sinh viên, giới hạn không được nhỏ hơn số này.");

            if (!ModelState.IsValid)
            {
                ViewBag.TaiHienTai = tai;
                await NapDropdownFormAsync(input.MaLinhVuc);
                return View(input);
            }

            gv.HoTen = input.HoTen;
            gv.HocVi = input.HocVi;
            gv.Email = input.Email;
            gv.SoDienThoai = input.SoDienThoai;
            gv.MaLinhVuc = input.MaLinhVuc;
            gv.SoSinhVienHuongDanToiDa = input.SoSinhVienHuongDanToiDa;
            gv.TrangThai = input.TrangThai;

            await _context.SaveChangesAsync();
            TempData["ThongBao"] = $"Đã cập nhật giảng viên \"{gv.HoTen}\".";
            return RedirectToAction(nameof(Index));
        }

        // ===== DELETE (POST): không xóa nếu đã có phân công (giữ lịch sử) =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var gv = await _context.GiangViens.FindAsync(id);
            if (gv == null) return NotFound();

            bool coPhanCong = await _context.PhanCongHuongDans.AnyAsync(p => p.MaGiangVien == id);
            if (coPhanCong)
            {
                TempData["Loi"] = $"Không thể xóa \"{gv.HoTen}\" vì đã có lịch sử phân công. " +
                                  "Hãy chuyển trạng thái sang Tạm ngừng hướng dẫn.";
                return RedirectToAction(nameof(Index));
            }

            _context.GiangViens.Remove(gv);
            await _context.SaveChangesAsync();
            TempData["ThongBao"] = $"Đã xóa giảng viên \"{gv.HoTen}\".";
            return RedirectToAction(nameof(Index));
        }

        // Tìm kiếm + lọc (dùng chung cho Index và ExportExcel)
        private IQueryable<GiangVienListItem> LocGiangVien(GiangVienFilterViewModel vm)
        {
            var query = _context.GiangViens.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(vm.Keyword))
            {
                var kw = vm.Keyword.Trim();
                query = query.Where(g => g.HoTen.Contains(kw) || g.Email.Contains(kw));
            }
            if (vm.MaLinhVuc.HasValue)
                query = query.Where(g => g.MaLinhVuc == vm.MaLinhVuc.Value);
            if (!string.IsNullOrEmpty(vm.TrangThai))
                query = query.Where(g => g.TrangThai == vm.TrangThai);

            var items = query.ToListItems();

            // Lọc theo tải tính từ phân công thực tế
            if (vm.ConTai == true)
                items = items.Where(x => x.TaiHienTai < x.SoSinhVienHuongDanToiDa);
            else if (vm.ConTai == false)
                items = items.Where(x => x.TaiHienTai >= x.SoSinhVienHuongDanToiDa);
            return items;
        }

        // ===== XUẤT EXCEL (nâng cao): xuất đúng danh sách đang lọc =====
        public async Task<IActionResult> ExportExcel(GiangVienFilterViewModel vm)
        {
            var ds = await LocGiangVien(vm).OrderBy(x => x.HoTen).ThenBy(x => x.MaGiangVien).ToListAsync();
            var bytes = ExcelHelper.XuatGiangVien(ds);
            return File(bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"DanhSachGiangVien_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
        }

        // ===== Hàm hỗ trợ =====

        private Task<int> DemTaiHienTaiAsync(int maGiangVien) =>
            _context.PhanCongHuongDans.CountAsync(p =>
                p.MaGiangVien == maGiangVien && p.TrangThai == TrangThaiConst.PhanCongTT.DangHuongDan);

        // Kiểm tra nghiệp vụ phụ thuộc DB (LINQ). maDangSua = null khi Create.
        private async Task KiemTraNghiepVuAsync(GiangVien gv, int? maDangSua)
        {
            gv.HoTen = (gv.HoTen ?? string.Empty).Trim();
            gv.Email = (gv.Email ?? string.Empty).Trim();

            if (!TrangThaiConst.GiangVienTT.TatCa.Contains(gv.TrangThai))
                ModelState.AddModelError(nameof(gv.TrangThai), "Trạng thái không hợp lệ.");

            // Lĩnh vực phải tồn tại và đang hoạt động (giữ nguyên lĩnh vực cũ khi sửa)
            int? lvCu = maDangSua == null ? null : await _context.GiangViens
                .Where(g => g.MaGiangVien == maDangSua).Select(g => (int?)g.MaLinhVuc).FirstOrDefaultAsync();
            int lv = gv.MaLinhVuc;
            bool lvHopLe = await _context.LinhVucs.AnyAsync(l => l.MaLinhVuc == lv && (l.TrangThai || l.MaLinhVuc == lvCu));
            if (!lvHopLe)
                ModelState.AddModelError(nameof(gv.MaLinhVuc), "Lĩnh vực không tồn tại hoặc đã ngừng hoạt động.");

            // Email không trùng (loại trừ chính bản ghi đang sửa)
            string email = gv.Email;
            bool trung = await _context.GiangViens.AnyAsync(g =>
                g.Email == email && (maDangSua == null || g.MaGiangVien != maDangSua));
            if (trung)
                ModelState.AddModelError(nameof(gv.Email), "Email này đã được dùng cho giảng viên khác.");
        }

        private async Task NapDropdownFormAsync(int? maLinhVucDangChon)
        {
            var linhVucs = await _context.LinhVucs.AsNoTracking()
                .Where(l => l.TrangThai || l.MaLinhVuc == maLinhVucDangChon)
                .OrderBy(l => l.TenLinhVuc)
                .ToListAsync();
            ViewBag.LinhVucs = new SelectList(linhVucs, "MaLinhVuc", "TenLinhVuc", maLinhVucDangChon);

            ViewBag.TrangThais = TrangThaiConst.GiangVienTT.TatCa
                .Select(t => new SelectListItem(TrangThaiConst.GiangVienTT.Nhan(t), t)).ToList();
        }
    }
}
