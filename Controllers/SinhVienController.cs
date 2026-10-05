// Họ và tên: (Điền họ tên của bạn)
// Mã sinh viên: (Điền mã sinh viên của bạn)
// Nội dung thực hiện: Module 3 - Quản lý sinh viên (CRUD dành cho Admin,
// xem/cập nhật thông tin cá nhân dành cho Sinh viên)

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDeTaiDoAn.Common;
using QuanLyDeTaiDoAn.Data;
using QuanLyDeTaiDoAn.Filters;
using QuanLyDeTaiDoAn.Models;
using QuanLyDeTaiDoAn.ViewModels;

namespace QuanLyDeTaiDoAn.Controllers
{
    [RequireLogin]
    public class SinhVienController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SinhVienController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // KHU VỰC DÀNH CHO ADMIN: quản lý toàn bộ sinh viên
        // =========================================================

        // GET: /SinhVien
        [RequireRole(AppConstants.VaiTro_Admin)]
        public async Task<IActionResult> Index(string tuKhoa)
        {
            var query = _context.SinhViens.AsQueryable();

            // Tìm kiếm cơ bản theo Mã sinh viên / Họ tên / Lớp (LINQ - Where)
            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                query = query.Where(sv =>
                    sv.MaSinhVien.Contains(tuKhoa) ||
                    sv.HoTen.Contains(tuKhoa) ||
                    sv.Lop.Contains(tuKhoa));
            }

            ViewBag.TuKhoa = tuKhoa;

            var danhSach = await query
                .OrderBy(sv => sv.HoTen)
                .ToListAsync();

            return View(danhSach);
        }

        // GET: /SinhVien/Details/22103100001
        [RequireRole(AppConstants.VaiTro_Admin)]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var sinhVien = await _context.SinhViens
                .Include(sv => sv.TaiKhoan)
                .FirstOrDefaultAsync(sv => sv.MaSinhVien == id);

            if (sinhVien == null) return NotFound();

            return View(sinhVien);
        }

        // GET: /SinhVien/Create
        [RequireRole(AppConstants.VaiTro_Admin)]
        public IActionResult Create()
        {
            return View(new SinhVienCreateViewModel { TrangThai = AppConstants.SinhVien_DangHoc });
        }

        // POST: /SinhVien/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireRole(AppConstants.VaiTro_Admin)]
        public async Task<IActionResult> Create(SinhVienCreateViewModel model)
        {
            // ---- Validate nghiệp vụ bằng LINQ (không trùng mã SV / tên đăng nhập / email) ----
            if (await _context.SinhViens.AnyAsync(sv => sv.MaSinhVien == model.MaSinhVien))
            {
                ModelState.AddModelError(nameof(model.MaSinhVien), "Mã sinh viên đã tồn tại.");
            }
            if (await _context.TaiKhoans.AnyAsync(tk => tk.TenDangNhap == model.TenDangNhap))
            {
                ModelState.AddModelError(nameof(model.TenDangNhap), "Tên đăng nhập đã tồn tại.");
            }
            if (await _context.SinhViens.AnyAsync(sv => sv.Email == model.Email))
            {
                ModelState.AddModelError(nameof(model.Email), "Email đã được sử dụng.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Tạo Tài khoản trước, sau đó tạo Sinh viên gắn với Tài khoản đó (1-1)
            var taiKhoan = new TaiKhoan
            {
                TenDangNhap = model.TenDangNhap,
                MatKhau = model.MatKhau,
                HoTen = model.HoTen,
                Email = model.Email,
                VaiTro = AppConstants.VaiTro_SinhVien,
                TrangThai = AppConstants.TaiKhoan_HoatDong
            };
            _context.TaiKhoans.Add(taiKhoan);
            await _context.SaveChangesAsync(); // Lưu để sinh ra MaTaiKhoan

            var sinhVien = new SinhVien
            {
                MaSinhVien = model.MaSinhVien,
                MaTaiKhoan = taiKhoan.MaTaiKhoan,
                HoTen = model.HoTen,
                NgaySinh = model.NgaySinh,
                Lop = model.Lop,
                Khoa = model.Khoa,
                ChuyenNganh = model.ChuyenNganh,
                Email = model.Email,
                SoDienThoai = model.SoDienThoai,
                TrangThai = model.TrangThai
            };
            _context.SinhViens.Add(sinhVien);
            await _context.SaveChangesAsync();

            TempData["ThongBao"] = "Thêm sinh viên thành công.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /SinhVien/Edit/22103100001
        [RequireRole(AppConstants.VaiTro_Admin)]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var sinhVien = await _context.SinhViens.FindAsync(id);
            if (sinhVien == null) return NotFound();

            return View(sinhVien);
        }

        // POST: /SinhVien/Edit/22103100001
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireRole(AppConstants.VaiTro_Admin)]
        public async Task<IActionResult> Edit(string id, SinhVien model)
        {
            if (id != model.MaSinhVien) return NotFound();

            // Email không được trùng với sinh viên KHÁC
            if (await _context.SinhViens.AnyAsync(sv => sv.Email == model.Email && sv.MaSinhVien != id))
            {
                ModelState.AddModelError(nameof(model.Email), "Email đã được sử dụng bởi sinh viên khác.");
            }

            // Không cho sửa các trường không thuộc form (MaTaiKhoan) qua Model Binding thất thường
            ModelState.Remove(nameof(model.MaTaiKhoan));
            ModelState.Remove(nameof(model.TaiKhoan));

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var sinhVien = await _context.SinhViens.FindAsync(id);
            if (sinhVien == null) return NotFound();

            sinhVien.HoTen = model.HoTen;
            sinhVien.NgaySinh = model.NgaySinh;
            sinhVien.Lop = model.Lop;
            sinhVien.Khoa = model.Khoa;
            sinhVien.ChuyenNganh = model.ChuyenNganh;
            sinhVien.Email = model.Email;
            sinhVien.SoDienThoai = model.SoDienThoai;
            sinhVien.TrangThai = model.TrangThai;

            await _context.SaveChangesAsync();

            TempData["ThongBao"] = "Cập nhật sinh viên thành công.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /SinhVien/Delete/22103100001
        [RequireRole(AppConstants.VaiTro_Admin)]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var sinhVien = await _context.SinhViens
                .Include(sv => sv.TaiKhoan)
                .FirstOrDefaultAsync(sv => sv.MaSinhVien == id);

            if (sinhVien == null) return NotFound();

            // Cảnh báo trước cho Admin biết nếu sinh viên đã có lịch sử đăng ký
            ViewBag.DaCoDangKy = await _context.DangKyDeTais.AnyAsync(dk => dk.MaSinhVien == id);

            return View(sinhVien);
        }

        // POST: /SinhVien/Delete/22103100001
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RequireRole(AppConstants.VaiTro_Admin)]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var sinhVien = await _context.SinhViens.FindAsync(id);
            if (sinhVien == null) return NotFound();

            // Không cho xóa nếu sinh viên đã có lịch sử đăng ký đề tài (giữ lịch sử - mục 9.2)
            bool daCoDangKy = await _context.DangKyDeTais.AnyAsync(dk => dk.MaSinhVien == id);
            if (daCoDangKy)
            {
                TempData["Loi"] = "Không thể xóa: sinh viên đã có lịch sử đăng ký đề tài. " +
                                   "Hãy chuyển trạng thái sang \"Ngừng học\" thay vì xóa.";
                return RedirectToAction(nameof(Index));
            }

            _context.SinhViens.Remove(sinhVien);
            await _context.SaveChangesAsync();

            TempData["ThongBao"] = "Xóa sinh viên thành công.";
            return RedirectToAction(nameof(Index));
        }

        // =========================================================
        // KHU VỰC DÀNH CHO SINH VIÊN: xem/sửa thông tin cá nhân của CHÍNH MÌNH
        // =========================================================

        // GET: /SinhVien/ThongTinCaNhan
        // Lưu ý bảo mật: MaSinhVien được xác định từ Session (qua MaTaiKhoan),
        // KHÔNG được truyền qua tham số URL để tránh xem/sửa dữ liệu người khác.
        [RequireRole(AppConstants.VaiTro_SinhVien)]
        public async Task<IActionResult> ThongTinCaNhan()
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            var sinhVien = await _context.SinhViens.FirstOrDefaultAsync(sv => sv.MaTaiKhoan == maTaiKhoan);
            if (sinhVien == null) return NotFound();

            var model = new ThongTinCaNhanViewModel
            {
                MaSinhVien = sinhVien.MaSinhVien,
                HoTen = sinhVien.HoTen,
                Lop = sinhVien.Lop,
                Khoa = sinhVien.Khoa,
                ChuyenNganh = sinhVien.ChuyenNganh,
                Email = sinhVien.Email,
                SoDienThoai = sinhVien.SoDienThoai
            };
            return View(model);
        }

        // POST: /SinhVien/ThongTinCaNhan
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireRole(AppConstants.VaiTro_SinhVien)]
        public async Task<IActionResult> ThongTinCaNhan(ThongTinCaNhanViewModel model)
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            var sinhVien = await _context.SinhViens.FirstOrDefaultAsync(sv => sv.MaTaiKhoan == maTaiKhoan);
            if (sinhVien == null) return NotFound();

            if (await _context.SinhViens.AnyAsync(sv => sv.Email == model.Email && sv.MaSinhVien != sinhVien.MaSinhVien))
            {
                ModelState.AddModelError(nameof(model.Email), "Email đã được sử dụng.");
            }

            if (!ModelState.IsValid)
            {
                model.MaSinhVien = sinhVien.MaSinhVien;
                return View(model);
            }

            // Sinh viên CHỈ được sửa Email và Số điện thoại (mục 7.1);
            // Họ tên/Lớp/Khoa/Chuyên ngành do Admin quản lý.
            sinhVien.Email = model.Email;
            sinhVien.SoDienThoai = model.SoDienThoai;
            await _context.SaveChangesAsync();

            TempData["ThongBao"] = "Cập nhật thông tin cá nhân thành công.";
            return RedirectToAction(nameof(ThongTinCaNhan));
        }
    }
}
