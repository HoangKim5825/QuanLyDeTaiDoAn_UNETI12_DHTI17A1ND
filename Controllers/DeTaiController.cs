// Họ và tên: <ĐIỀN TÊN>
// Mã sinh viên: <ĐIỀN MÃ SV>
// Nội dung thực hiện: Quản lý đề tài - CRUD, tìm kiếm, lọc, sắp xếp, phân trang (Module 2)
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
    [YeuCauDangNhap]
    public class DeTaiController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        // Trạng thái được chọn tay. "DuSoLuong" KHÔNG cho chọn vì được suy ra từ SoChoCon.
        private static readonly string[] TrangThaiChoPhep =
        {
            TrangThaiConst.DeTaiTT.MoDangKy,
            TrangThaiConst.DeTaiTT.TamKhoa,
            TrangThaiConst.DeTaiTT.KetThuc
        };

        public DeTaiController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // =====================================================================
        // INDEX: Truy vấn -> Tìm kiếm -> Lọc -> Sắp xếp -> Phân trang -> Hiển thị
        // =====================================================================
        public async Task<IActionResult> Index(DeTaiFilterViewModel vm)
        {
            var items = LocDeTai(vm);
            vm.KetQua = await items.ToPagedResultAsync(vm.Page, vm.PageSize);
            vm.Page = vm.KetQua.Page;

            // AJAX: chỉ trả phần danh sách + phân trang, không trả cả trang
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_DanhSach", vm);

            await NapDropdownLocAsync(vm);
            return View(vm);
        }

        // =====================================================================
        // DETAILS
        // =====================================================================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var deTai = await _context.DeTais.AsNoTracking()
                .Include(d => d.LinhVuc)
                .FirstOrDefaultAsync(d => d.MaDeTai == id);
            if (deTai == null) return NotFound();

            // Dùng CHUNG công thức số chỗ với danh sách
            var so = await _context.DeTais.Where(d => d.MaDeTai == id)
                .ToListItems().FirstAsync();

            return View(new DeTaiDetailsViewModel
            {
                DeTai = deTai,
                SoDaDangKy = so.SoDaDangKy,
                SoChoCon = so.SoChoCon
            });
        }

        // =====================================================================
        // CREATE
        // =====================================================================
        [YeuCauAdmin]
        public async Task<IActionResult> Create()
        {
            var deTai = new DeTai
            {
                NamHoc = DateTime.Now.Year,
                HocKy = "HK1",
                SoSinhVienToiDa = 1,
                TrangThai = TrangThaiConst.DeTaiTT.MoDangKy,
                MaLinhVuc = await _context.LinhVucs.Where(l => l.TrangThai)
                    .OrderBy(l => l.TenLinhVuc).Select(l => l.MaLinhVuc).FirstOrDefaultAsync()
            };
            await NapDropdownFormAsync(deTai.MaLinhVuc);
            return View(deTai);
        }

        [YeuCauAdmin]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("TenDeTai,MaLinhVuc,MoTa,YeuCau,SoSinhVienToiDa,NamHoc,HocKy,TrangThai")] DeTai deTai,
            IFormFile? fileMoTa)
        {
            await KiemTraNghiepVuAsync(deTai, null);
            await KiemTraFileAsync(fileMoTa);

            if (!ModelState.IsValid)
            {
                await NapDropdownFormAsync(deTai.MaLinhVuc);   // nạp lại dropdown khi lỗi
                return View(deTai);
            }

            if (fileMoTa is { Length: > 0 })
            {
                deTai.TenFileMoTa = FileMoTaHelper.TenHienThi(fileMoTa.FileName);
                deTai.TenFileLuu = await FileMoTaHelper.LuuAsync(fileMoTa, _env);
            }

            _context.DeTais.Add(deTai);
            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateException ex)
            {
                FileMoTaHelper.Xoa(_env, deTai.TenFileLuu);   // lỗi DB thì không để lại file mồ côi
                if (!LaLoiTrung(ex)) throw;
                // Hai người lưu cùng lúc: vượt qua kiểm tra LINQ nhưng bị unique index chặn
                ModelState.AddModelError(nameof(deTai.TenDeTai), "Đã có đề tài cùng tên trong năm học và học kỳ này.");
                deTai.TenFileMoTa = null;
                deTai.TenFileLuu = null;
                await NapDropdownFormAsync(deTai.MaLinhVuc);
                return View(deTai);
            }
            TempData["ThongBao"] = $"Đã thêm đề tài \"{deTai.TenDeTai}\".";
            return RedirectToAction(nameof(Index));
        }

        // =====================================================================
        // EDIT
        // =====================================================================
        [YeuCauAdmin]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var deTai = await _context.DeTais.AsNoTracking().FirstOrDefaultAsync(d => d.MaDeTai == id);
            if (deTai == null) return NotFound();

            // "DuSoLuong" là trạng thái suy ra, không có trong ô chọn
            if (deTai.TrangThai == TrangThaiConst.DeTaiTT.DuSoLuong)
                deTai.TrangThai = TrangThaiConst.DeTaiTT.MoDangKy;

            ViewBag.SoDaDuyet = await _context.DangKyDeTais.DangKyHopLe().CountAsync(x => x.MaDeTai == id);
            await NapDropdownFormAsync(deTai.MaLinhVuc);
            return View(deTai);
        }

        [YeuCauAdmin]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("MaDeTai,TenDeTai,MaLinhVuc,MoTa,YeuCau,SoSinhVienToiDa,NamHoc,HocKy,TrangThai")] DeTai input,
            IFormFile? fileMoTa, bool xoaFile = false)
        {
            if (id != input.MaDeTai) return NotFound();

            var deTai = await _context.DeTais.FindAsync(id);
            if (deTai == null) return NotFound();

            await KiemTraNghiepVuAsync(input, id);
            await KiemTraFileAsync(fileMoTa);

            // Không cho giảm sức chứa thấp hơn số đăng ký đã duyệt
            int soDaDuyet = await _context.DangKyDeTais.DangKyHopLe().CountAsync(x => x.MaDeTai == id);
            if (input.SoSinhVienToiDa < soDaDuyet)
                ModelState.AddModelError(nameof(input.SoSinhVienToiDa),
                    $"Đề tài đã có {soDaDuyet} đăng ký được duyệt, số sinh viên tối đa không được nhỏ hơn số này.");

            if (!ModelState.IsValid)
            {
                ViewBag.SoDaDuyet = soDaDuyet;
                input.TenFileMoTa = deTai.TenFileMoTa;   // để View còn hiển thị file hiện tại
                await NapDropdownFormAsync(input.MaLinhVuc);
                return View(input);
            }

            // Chỉ gán các trường được phép sửa
            deTai.TenDeTai = input.TenDeTai;
            deTai.MaLinhVuc = input.MaLinhVuc;
            deTai.MoTa = input.MoTa;
            deTai.YeuCau = input.YeuCau;
            deTai.SoSinhVienToiDa = input.SoSinhVienToiDa;
            deTai.NamHoc = input.NamHoc;
            deTai.HocKy = input.HocKy;
            deTai.TrangThai = input.TrangThai;

            string? fileCu = deTai.TenFileLuu;
            string? tenGocCu = deTai.TenFileMoTa;
            string? fileMoi = null;
            if (fileMoTa is { Length: > 0 })             // thay file
            {
                fileMoi = await FileMoTaHelper.LuuAsync(fileMoTa, _env);
                deTai.TenFileMoTa = FileMoTaHelper.TenHienThi(fileMoTa.FileName);
                deTai.TenFileLuu = fileMoi;
            }
            else if (xoaFile)                            // chỉ xóa file
            {
                deTai.TenFileMoTa = null;
                deTai.TenFileLuu = null;
            }

            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateException ex)
            {
                FileMoTaHelper.Xoa(_env, fileMoi);
                if (!LaLoiTrung(ex)) throw;
                ModelState.AddModelError(nameof(input.TenDeTai), "Đã có đề tài cùng tên trong năm học và học kỳ này.");
                input.TenFileMoTa = tenGocCu;
                ViewBag.SoDaDuyet = soDaDuyet;
                await NapDropdownFormAsync(input.MaLinhVuc);
                return View(input);
            }

            // Lưu DB thành công rồi mới xóa file cũ trên đĩa
            if (fileCu != null && fileCu != deTai.TenFileLuu) FileMoTaHelper.Xoa(_env, fileCu);
            TempData["ThongBao"] = $"Đã cập nhật đề tài \"{deTai.TenDeTai}\".";
            return RedirectToAction(nameof(Index));
        }

        // =====================================================================
        // DELETE (POST) - không xóa nếu đã có đăng ký để bảo toàn lịch sử
        // =====================================================================
        [YeuCauAdmin]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var deTai = await _context.DeTais.FindAsync(id);
            if (deTai == null) return NotFound();

            bool coDangKy = await _context.DangKyDeTais.AnyAsync(x => x.MaDeTai == id);
            if (coDangKy)
            {
                TempData["Loi"] = $"Không thể xóa \"{deTai.TenDeTai}\" vì đã có đăng ký liên quan. " +
                                  "Hãy chuyển trạng thái sang Tạm khóa hoặc Kết thúc.";
                return RedirectToAction(nameof(Index));
            }

            _context.DeTais.Remove(deTai);
            await _context.SaveChangesAsync();
            FileMoTaHelper.Xoa(_env, deTai.TenFileLuu);
            TempData["ThongBao"] = $"Đã xóa đề tài \"{deTai.TenDeTai}\".";
            return RedirectToAction(nameof(Index));
        }

        // =====================================================================
        // Hàm hỗ trợ
        // =====================================================================

        // Vi phạm unique index của SQL Server (mã lỗi 2601 hoặc 2627)
        private static bool LaLoiTrung(DbUpdateException ex) =>
            ex.InnerException is Microsoft.Data.SqlClient.SqlException { Number: 2601 or 2627 };

        private async Task KiemTraFileAsync(IFormFile? file)
        {
            if (file == null || file.Length == 0) return;
            var loi = await FileMoTaHelper.KiemTraAsync(file);
            if (loi != null) ModelState.AddModelError("fileMoTa", loi);
        }

        // Kiểm tra nghiệp vụ phụ thuộc DB (LINQ). maDangSua = null khi Create.
        private async Task KiemTraNghiepVuAsync(DeTai dt, int? maDangSua)
        {
            dt.TenDeTai = (dt.TenDeTai ?? string.Empty).Trim();

            if (dt.NamHoc < 2000 || dt.NamHoc > 2100)
                ModelState.AddModelError(nameof(dt.NamHoc), "Năm học phải từ 2000 đến 2100.");

            if (dt.HocKy != "HK1" && dt.HocKy != "HK2")
                ModelState.AddModelError(nameof(dt.HocKy), "Học kỳ không hợp lệ.");

            if (!TrangThaiChoPhep.Contains(dt.TrangThai))
                ModelState.AddModelError(nameof(dt.TrangThai), "Trạng thái không hợp lệ.");

            // Lĩnh vực phải tồn tại và đang hoạt động (giữ nguyên lĩnh vực cũ khi sửa)
            int? lvCu = maDangSua == null ? null : await _context.DeTais
                .Where(d => d.MaDeTai == maDangSua).Select(d => (int?)d.MaLinhVuc).FirstOrDefaultAsync();
            int lv = dt.MaLinhVuc;
            bool lvHopLe = await _context.LinhVucs.AnyAsync(l => l.MaLinhVuc == lv && (l.TrangThai || l.MaLinhVuc == lvCu));
            if (!lvHopLe)
                ModelState.AddModelError(nameof(dt.MaLinhVuc), "Lĩnh vực không tồn tại hoặc đã ngừng hoạt động.");

            // Trùng tên trong cùng năm học + học kỳ
            string ten = dt.TenDeTai;
            int nam = dt.NamHoc;
            string hk = dt.HocKy;
            bool trung = await _context.DeTais.AnyAsync(d =>
                d.TenDeTai == ten && d.NamHoc == nam && d.HocKy == hk
                && (maDangSua == null || d.MaDeTai != maDangSua));
            if (trung)
                ModelState.AddModelError(nameof(dt.TenDeTai),
                    "Đã có đề tài cùng tên trong năm học và học kỳ này.");
        }

        private async Task NapDropdownFormAsync(int? maLinhVucDangChon)
        {
            var linhVucs = await _context.LinhVucs.AsNoTracking()
                .Where(l => l.TrangThai || l.MaLinhVuc == maLinhVucDangChon)
                .OrderBy(l => l.TenLinhVuc)
                .ToListAsync();
            ViewBag.LinhVucs = new SelectList(linhVucs, "MaLinhVuc", "TenLinhVuc", maLinhVucDangChon);

            ViewBag.HocKys = new List<SelectListItem> { new("Học kỳ 1", "HK1"), new("Học kỳ 2", "HK2") };

            ViewBag.TrangThais = TrangThaiChoPhep
                .Select(t => new SelectListItem(TrangThaiConst.DeTaiTT.Nhan(t), t)).ToList();
        }

        // Truy vấn -> Tìm kiếm -> Lọc -> Sắp xếp (CHƯA phân trang).
        // Dùng chung cho Index (phân trang) và ExportExcel (lấy toàn bộ kết quả đang lọc).
        private IQueryable<DeTaiListItem> LocDeTai(DeTaiFilterViewModel vm)
        {
            var query = _context.DeTais.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(vm.Keyword))
            {
                var kw = vm.Keyword.Trim();
                query = query.Where(d => d.TenDeTai.Contains(kw)
                                      || (d.MoTa != null && d.MoTa.Contains(kw))
                                      || (d.YeuCau != null && d.YeuCau.Contains(kw)));
            }

            if (vm.MaLinhVuc.HasValue)
                query = query.Where(d => d.MaLinhVuc == vm.MaLinhVuc.Value);
            if (vm.NamHoc.HasValue)
                query = query.Where(d => d.NamHoc == vm.NamHoc.Value);
            if (!string.IsNullOrEmpty(vm.HocKy))
                query = query.Where(d => d.HocKy == vm.HocKy);

            var items = query.ToListItems();

            if (vm.ConCho == true)
                items = items.Where(x => x.SoChoCon > 0);
            else if (vm.ConCho == false)
                items = items.Where(x => x.SoChoCon <= 0);

            if (!string.IsNullOrEmpty(vm.TrangThai))
            {
                if (vm.TrangThai == TrangThaiConst.DeTaiTT.DuSoLuong)
                    items = items.Where(x => x.TrangThai == TrangThaiConst.DeTaiTT.DuSoLuong
                        || (x.TrangThai == TrangThaiConst.DeTaiTT.MoDangKy && x.SoChoCon <= 0));
                else
                    items = items.Where(x => x.TrangThai == vm.TrangThai);
            }

            items = items.ApplySort(vm.SortOrder);
            return items;
        }

        // ===== TẢI FILE MÔ TẢ (nâng cao) =====
        // File nằm ngoài wwwroot, chỉ tải được qua action này và phải đăng nhập.
        public async Task<IActionResult> TaiFile(int id)
        {
            var f = await _context.DeTais.AsNoTracking()
                .Where(d => d.MaDeTai == id)
                .Select(d => new { d.TenFileMoTa, d.TenFileLuu })
                .FirstOrDefaultAsync();
            if (f?.TenFileLuu == null) return NotFound();

            var path = FileMoTaHelper.DuongDan(_env, f.TenFileLuu);
            if (!System.IO.File.Exists(path)) return NotFound();

            Response.Headers["X-Content-Type-Options"] = "nosniff";
            return PhysicalFile(path, FileMoTaHelper.LoaiNoiDung(f.TenFileLuu), f.TenFileMoTa ?? f.TenFileLuu);
        }

        // ===== XUẤT EXCEL (nâng cao): xuất đúng danh sách đang tìm kiếm/lọc/sắp xếp =====
        [YeuCauAdmin]
        public async Task<IActionResult> ExportExcel(DeTaiFilterViewModel vm)
        {
            var ds = await LocDeTai(vm).ToListAsync();
            var bytes = ExcelHelper.XuatDeTai(ds);
            return File(bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"DanhSachDeTai_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
        }

        private async Task NapDropdownLocAsync(DeTaiFilterViewModel vm)
        {
            vm.LinhVucs = await _context.LinhVucs.AsNoTracking()
                .OrderBy(l => l.TenLinhVuc)
                .Select(l => new SelectListItem(l.TenLinhVuc, l.MaLinhVuc.ToString()))
                .ToListAsync();

            vm.NamHocs = (await _context.DeTais.AsNoTracking()
                .Select(d => d.NamHoc).Distinct().OrderByDescending(n => n).ToListAsync())
                .Select(n => new SelectListItem(n.ToString(), n.ToString())).ToList();

            vm.HocKys = (await _context.DeTais.AsNoTracking()
                .Select(d => d.HocKy).Distinct().OrderBy(h => h).ToListAsync())
                .Select(h => new SelectListItem(h, h)).ToList();

            vm.TrangThais = TrangThaiConst.DeTaiTT.TatCa
                .Select(t => new SelectListItem(TrangThaiConst.DeTaiTT.Nhan(t), t)).ToList();

            vm.CacKieuSapXep = new()
            {
                new("Tên A → Z", "ten_asc"),
                new("Tên Z → A", "ten_desc"),
                new("Số SV tối đa tăng dần", "max_asc"),
                new("Số SV tối đa giảm dần", "max_desc"),
                new("Số chỗ còn tăng dần", "cho_asc"),
                new("Số chỗ còn giảm dần", "cho_desc"),
            };
        }
    }
}
