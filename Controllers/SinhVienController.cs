// Họ và tên: <ĐIỀN TÊN>
// Mã sinh viên: <ĐIỀN MÃ SV>
// Nội dung thực hiện: Module 3 - Quản lý sinh viên (Admin: CRUD, tìm kiếm, lọc, phân trang)
// và hồ sơ cá nhân (Sinh viên chỉ xem/sửa thông tin được phép của chính mình)
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyDeTaiDoAn.Data;
using QuanLyDeTaiDoAn.Extensions;
using QuanLyDeTaiDoAn.Filters;
using QuanLyDeTaiDoAn.Helpers;
using QuanLyDeTaiDoAn.Models;
using QuanLyDeTaiDoAn.ViewModels;

namespace QuanLyDeTaiDoAn.Controllers
{
    public class SinhVienController : Controller
    {
        private const int PageSize = 10;
        private readonly ApplicationDbContext _context;

        public SinhVienController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =====================================================================
        // ADMIN: Truy vấn -> Tìm kiếm -> Lọc -> Sắp xếp -> Phân trang
        // =====================================================================
        [YeuCauAdmin]
        public async Task<IActionResult> Index(SinhVienFilterViewModel vm)
        {
            var query = _context.SinhViens.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(vm.Keyword))
            {
                var kw = vm.Keyword.Trim();
                bool laSo = int.TryParse(kw, out int ma);
                query = query.Where(s => s.HoTen.Contains(kw) || s.Email.Contains(kw)
                                      || (laSo && s.MaSinhVien == ma));
            }
            if (!string.IsNullOrEmpty(vm.Lop))
                query = query.Where(s => s.Lop == vm.Lop);
            if (vm.TrangThai.HasValue)
                query = query.Where(s => s.TrangThai == vm.TrangThai.Value);

            vm.KetQua = await query
                .OrderBy(s => s.HoTen).ThenBy(s => s.MaSinhVien)
                .Select(s => new SinhVienListItem
                {
                    MaSinhVien = s.MaSinhVien,
                    HoTen = s.HoTen,
                    Lop = s.Lop,
                    Khoa = s.Khoa,
                    Email = s.Email,
                    SoDienThoai = s.SoDienThoai,
                    TrangThai = s.TrangThai,
                    TenDangNhap = s.TaiKhoan!.TenDangNhap,
                    SoDangKy = s.DanhSachDangKy.Count()
                })
                .ToPagedResultAsync(vm.Page, PageSize);
            vm.Page = vm.KetQua.Page;

            vm.DanhSachLop = await _context.SinhViens.AsNoTracking()
                .Where(s => s.Lop != null).Select(s => s.Lop!).Distinct().OrderBy(l => l).ToListAsync();

            return View(vm);
        }

        [YeuCauAdmin]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var sv = await _context.SinhViens.AsNoTracking()
                .Include(s => s.TaiKhoan)
                .FirstOrDefaultAsync(s => s.MaSinhVien == id);
            if (sv == null) return NotFound();

            ViewBag.SoDangKy = await _context.DangKyDeTais.CountAsync(d => d.MaSinhVien == id);
            return View(sv);
        }

        // ===== CREATE =====
        [YeuCauAdmin]
        public async Task<IActionResult> Create()
        {
            var sv = new SinhVien
            {
                NgaySinh = new DateTime(2004, 1, 1),
                Khoa = "Công nghệ thông tin",
                TrangThai = true
            };
            await NapDropdownTaiKhoanAsync(null);
            return View(sv);
        }

        [YeuCauAdmin]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("MaTaiKhoan,HoTen,NgaySinh,Lop,Khoa,ChuyenNganh,Email,SoDienThoai,TrangThai")] SinhVien sv)
        {
            await KiemTraNghiepVuAsync(sv, null);

            if (!ModelState.IsValid)
            {
                await NapDropdownTaiKhoanAsync(sv.MaTaiKhoan);
                return View(sv);
            }

            _context.SinhViens.Add(sv);
            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateException ex) when (LaLoiTrung(ex))
            {
                ModelState.AddModelError(string.Empty, "Email hoặc tài khoản này đã được dùng cho sinh viên khác.");
                await NapDropdownTaiKhoanAsync(sv.MaTaiKhoan);
                return View(sv);
            }

            TempData["ThongBao"] = $"Đã thêm sinh viên \"{sv.HoTen}\".";
            return RedirectToAction(nameof(Index));
        }

        // ===== EDIT (không cho đổi tài khoản liên kết) =====
        [YeuCauAdmin]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var sv = await _context.SinhViens.AsNoTracking().FirstOrDefaultAsync(s => s.MaSinhVien == id);
            if (sv == null) return NotFound();

            await NapTenDangNhapAsync(sv.MaTaiKhoan);
            return View(sv);
        }

        [YeuCauAdmin]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("MaSinhVien,HoTen,NgaySinh,Lop,Khoa,ChuyenNganh,Email,SoDienThoai,TrangThai")] SinhVien input)
        {
            if (id != input.MaSinhVien) return NotFound();
            var sv = await _context.SinhViens.FindAsync(id);
            if (sv == null) return NotFound();

            await KiemTraNghiepVuAsync(input, id);

            if (!ModelState.IsValid)
            {
                await NapTenDangNhapAsync(sv.MaTaiKhoan);
                return View(input);
            }

            // Chỉ gán các trường được phép sửa (MaTaiKhoan giữ nguyên)
            sv.HoTen = input.HoTen;
            sv.NgaySinh = input.NgaySinh;
            sv.Lop = input.Lop;
            sv.Khoa = input.Khoa;
            sv.ChuyenNganh = input.ChuyenNganh;
            sv.Email = input.Email;
            sv.SoDienThoai = input.SoDienThoai;
            sv.TrangThai = input.TrangThai;

            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateException ex) when (LaLoiTrung(ex))
            {
                ModelState.AddModelError(nameof(input.Email), "Email này đã được dùng cho sinh viên khác.");
                await NapTenDangNhapAsync(sv.MaTaiKhoan);
                return View(input);
            }

            TempData["ThongBao"] = $"Đã cập nhật sinh viên \"{sv.HoTen}\".";
            return RedirectToAction(nameof(Index));
        }

        // ===== DELETE (POST): không xóa nếu đã có đăng ký để giữ lịch sử =====
        [YeuCauAdmin]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var sv = await _context.SinhViens.FindAsync(id);
            if (sv == null) return NotFound();

            if (await _context.DangKyDeTais.AnyAsync(d => d.MaSinhVien == id))
            {
                TempData["Loi"] = $"Không thể xóa \"{sv.HoTen}\" vì đã có lịch sử đăng ký đề tài. " +
                                  "Hãy bỏ chọn \"Đang hoạt động\" để ngừng cho đăng ký.";
                return RedirectToAction(nameof(Index));
            }

            _context.SinhViens.Remove(sv);
            await _context.SaveChangesAsync();
            TempData["ThongBao"] = $"Đã xóa sinh viên \"{sv.HoTen}\".";
            return RedirectToAction(nameof(Index));
        }

        // =====================================================================
        // SINH VIÊN: hồ sơ cá nhân. Danh tính lấy từ Session, KHÔNG nhận ID từ URL/form.
        // =====================================================================
        [YeuCauDangNhap]
        [HttpGet]
        public async Task<IActionResult> HoSo()
        {
            if (!LaSinhVien) return RedirectToAction("Index", "Home");

            var vm = await LayHoSoAsync();
            if (vm == null)
            {
                TempData["Loi"] = "Tài khoản của bạn chưa có hồ sơ sinh viên. Vui lòng liên hệ quản trị viên.";
                return RedirectToAction("Index", "Home");
            }
            return View(vm);
        }

        [YeuCauDangNhap]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HoSo([Bind("Email,SoDienThoai")] SinhVienHoSoViewModel input)
        {
            if (!LaSinhVien) return RedirectToAction("Index", "Home");

            int maTk = HttpContext.Session.GetInt32(YeuCauDangNhapAttribute.SessionMaTaiKhoan)!.Value;
            var sv = await _context.SinhViens.Include(s => s.TaiKhoan).FirstOrDefaultAsync(s => s.MaTaiKhoan == maTk);
            if (sv == null) return NotFound();

            input.Email = (input.Email ?? string.Empty).Trim();
            string email = input.Email;
            if (await _context.SinhViens.AnyAsync(s => s.Email == email && s.MaSinhVien != sv.MaSinhVien))
                ModelState.AddModelError(nameof(input.Email), "Email này đã được dùng cho sinh viên khác.");

            if (!ModelState.IsValid)
            {
                NapHienThi(input, sv);
                return View(input);
            }

            sv.Email = input.Email;
            sv.SoDienThoai = string.IsNullOrWhiteSpace(input.SoDienThoai) ? null : input.SoDienThoai.Trim();

            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateException ex) when (LaLoiTrung(ex))
            {
                ModelState.AddModelError(nameof(input.Email), "Email này đã được dùng cho sinh viên khác.");
                NapHienThi(input, sv);
                return View(input);
            }

            TempData["ThongBao"] = "Đã cập nhật thông tin cá nhân.";
            return RedirectToAction(nameof(HoSo));
        }

        // =====================================================================
        // Hàm hỗ trợ
        // =====================================================================
        private bool LaSinhVien =>
            HttpContext.Session.GetString(YeuCauDangNhapAttribute.SessionVaiTro) == VaiTroTaiKhoan.SinhVien;

        private static bool LaLoiTrung(DbUpdateException ex) =>
            ex.InnerException is Microsoft.Data.SqlClient.SqlException { Number: 2601 or 2627 };

        private async Task<SinhVienHoSoViewModel?> LayHoSoAsync()
        {
            int maTk = HttpContext.Session.GetInt32(YeuCauDangNhapAttribute.SessionMaTaiKhoan)!.Value;
            return await _context.SinhViens.AsNoTracking()
                .Where(s => s.MaTaiKhoan == maTk)
                .Select(s => new SinhVienHoSoViewModel
                {
                    MaSinhVien = s.MaSinhVien,
                    TenDangNhap = s.TaiKhoan!.TenDangNhap,
                    HoTen = s.HoTen,
                    NgaySinh = s.NgaySinh,
                    Lop = s.Lop,
                    Khoa = s.Khoa,
                    ChuyenNganh = s.ChuyenNganh,
                    Email = s.Email,
                    SoDienThoai = s.SoDienThoai
                })
                .FirstOrDefaultAsync();
        }

        private static void NapHienThi(SinhVienHoSoViewModel vm, SinhVien sv)
        {
            vm.MaSinhVien = sv.MaSinhVien;
            vm.TenDangNhap = sv.TaiKhoan?.TenDangNhap ?? string.Empty;
            vm.HoTen = sv.HoTen;
            vm.NgaySinh = sv.NgaySinh;
            vm.Lop = sv.Lop;
            vm.Khoa = sv.Khoa;
            vm.ChuyenNganh = sv.ChuyenNganh;
        }

        // Kiểm tra nghiệp vụ phụ thuộc DB (LINQ). maDangSua = null khi Create.
        private async Task KiemTraNghiepVuAsync(SinhVien sv, int? maDangSua)
        {
            sv.HoTen = (sv.HoTen ?? string.Empty).Trim();
            sv.Email = (sv.Email ?? string.Empty).Trim();
            sv.Lop = string.IsNullOrWhiteSpace(sv.Lop) ? null : sv.Lop.Trim();

            if (sv.NgaySinh.Date >= DateTime.Today || sv.NgaySinh.Year < 1950)
                ModelState.AddModelError(nameof(sv.NgaySinh), "Ngày sinh không hợp lệ.");

            if (maDangSua == null)
            {
                int tk = sv.MaTaiKhoan;
                var taiKhoan = await _context.TaiKhoans.AsNoTracking()
                    .Where(t => t.MaTaiKhoan == tk).Select(t => new { t.VaiTro }).FirstOrDefaultAsync();

                if (taiKhoan == null || taiKhoan.VaiTro != VaiTroTaiKhoan.SinhVien)
                    ModelState.AddModelError(nameof(sv.MaTaiKhoan), "Vui lòng chọn tài khoản có vai trò Sinh viên.");
                else if (await _context.SinhViens.AnyAsync(s => s.MaTaiKhoan == tk))
                    ModelState.AddModelError(nameof(sv.MaTaiKhoan), "Tài khoản này đã có hồ sơ sinh viên.");
            }

            string email = sv.Email;
            bool trung = await _context.SinhViens.AnyAsync(s =>
                s.Email == email && (maDangSua == null || s.MaSinhVien != maDangSua));
            if (trung)
                ModelState.AddModelError(nameof(sv.Email), "Email này đã được dùng cho sinh viên khác.");
        }

        // Chỉ liệt kê tài khoản Sinh viên CHƯA có hồ sơ
        private async Task NapDropdownTaiKhoanAsync(int? maDangChon)
        {
            var ds = await _context.TaiKhoans.AsNoTracking()
                .Where(t => t.VaiTro == VaiTroTaiKhoan.SinhVien
                         && (!_context.SinhViens.Any(s => s.MaTaiKhoan == t.MaTaiKhoan) || t.MaTaiKhoan == maDangChon))
                .OrderBy(t => t.TenDangNhap)
                .Select(t => new { t.MaTaiKhoan, t.TenDangNhap, t.HoTen })
                .ToListAsync();

            ViewBag.TaiKhoans = ds
                .Select(t => new SelectListItem($"{t.TenDangNhap} - {t.HoTen}", t.MaTaiKhoan.ToString()))
                .ToList();
        }

        private async Task NapTenDangNhapAsync(int maTaiKhoan)
        {
            ViewBag.TenDangNhap = await _context.TaiKhoans.AsNoTracking()
                .Where(t => t.MaTaiKhoan == maTaiKhoan).Select(t => t.TenDangNhap).FirstOrDefaultAsync();
        }
    }
}
