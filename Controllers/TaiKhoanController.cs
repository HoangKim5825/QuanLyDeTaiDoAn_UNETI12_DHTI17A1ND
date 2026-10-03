// Họ và tên: Nguyễn Minh Đức
// Mã sinh viên: [23203100078]
// Nội dung thực hiện: Module 1 - ViewModel cho đăng nhập và quản lý tài khoản

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDeTaiDoAn.Data;
using QuanLyDeTaiDoAn.Filters;
using QuanLyDeTaiDoAn.Helpers;
using QuanLyDeTaiDoAn.Models;
using QuanLyDeTaiDoAn.ViewModels;

namespace QuanLyDeTaiDoAn.Controllers
{
    [YeuCauAdmin]
    public class TaiKhoanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaiKhoanController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string? keyword)
        {
            var query = _context.TaiKhoans.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                query = query.Where(t =>
                    t.TenDangNhap.Contains(keyword) ||
                    t.HoTen.Contains(keyword) ||
                    t.Email.Contains(keyword));
            }

            ViewBag.Keyword = keyword;
            var list = query.OrderBy(t => t.HoTen).ToList();
            return View(list);
        }

        public IActionResult Details(int id)
        {
            var taiKhoan = _context.TaiKhoans.FirstOrDefault(t => t.MaTaiKhoan == id);
            if (taiKhoan == null) return NotFound();
            return View(taiKhoan);
        }

        public IActionResult Create()
        {
            return View(new TaiKhoanCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TaiKhoanCreateViewModel model)
        {
            if (_context.TaiKhoans.Any(t => t.TenDangNhap == model.TenDangNhap))
                ModelState.AddModelError(nameof(model.TenDangNhap), "Tên đăng nhập đã tồn tại.");

            if (model.VaiTro != VaiTroTaiKhoan.Admin && model.VaiTro != VaiTroTaiKhoan.SinhVien)
                ModelState.AddModelError(nameof(model.VaiTro), "Vai trò không hợp lệ.");

            if (!ModelState.IsValid)
                return View(model);

            // Lưu ý: tạo tài khoản ở đây KHÔNG tự tạo hồ sơ SinhVien tương ứng
            // (hồ sơ SinhVien thuộc phạm vi Module 3) - chỉ tạo đúng 1 dòng TaiKhoan.
            var taiKhoan = new TaiKhoan
            {
                TenDangNhap = model.TenDangNhap,
                MatKhau = model.MatKhau,
                HoTen = model.HoTen,
                Email = model.Email,
                VaiTro = model.VaiTro,
                TrangThai = model.TrangThai
            };

            _context.TaiKhoans.Add(taiKhoan);
            _context.SaveChanges();

            TempData["Success"] = "Thêm tài khoản thành công.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var taiKhoan = _context.TaiKhoans.FirstOrDefault(t => t.MaTaiKhoan == id);
            if (taiKhoan == null) return NotFound();

            var vm = new TaiKhoanEditViewModel
            {
                MaTaiKhoan = taiKhoan.MaTaiKhoan,
                TenDangNhap = taiKhoan.TenDangNhap,
                HoTen = taiKhoan.HoTen,
                Email = taiKhoan.Email,
                VaiTro = taiKhoan.VaiTro,
                TrangThai = taiKhoan.TrangThai
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, TaiKhoanEditViewModel model)
        {
            if (id != model.MaTaiKhoan) return NotFound();

            if (model.VaiTro != VaiTroTaiKhoan.Admin && model.VaiTro != VaiTroTaiKhoan.SinhVien)
                ModelState.AddModelError(nameof(model.VaiTro), "Vai trò không hợp lệ.");

            // Nếu tài khoản đã có hồ sơ SinhVien liên kết, không cho đổi vai trò sang Admin
            // (tránh làm sai lệch dữ liệu Module 3 đang tham chiếu tài khoản này là Sinh viên)
            var daLaSinhVien = _context.SinhViens.Any(s => s.MaTaiKhoan == id);
            if (daLaSinhVien && model.VaiTro != VaiTroTaiKhoan.SinhVien)
                ModelState.AddModelError(nameof(model.VaiTro), "Tài khoản đã gắn hồ sơ Sinh viên, không thể đổi sang vai trò khác.");

            if (!ModelState.IsValid)
                return View(model);

            var taiKhoan = _context.TaiKhoans.FirstOrDefault(t => t.MaTaiKhoan == id);
            if (taiKhoan == null) return NotFound();

            taiKhoan.HoTen = model.HoTen;
            taiKhoan.Email = model.Email;
            taiKhoan.VaiTro = model.VaiTro;
            taiKhoan.TrangThai = model.TrangThai;

            _context.SaveChanges();
            TempData["Success"] = "Cập nhật tài khoản thành công.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleKhoa(int id)
        {
            var taiKhoan = _context.TaiKhoans.FirstOrDefault(t => t.MaTaiKhoan == id);
            if (taiKhoan == null) return NotFound();

            taiKhoan.TrangThai = !taiKhoan.TrangThai;
            _context.SaveChanges();

            TempData["Success"] = taiKhoan.TrangThai ? "Đã mở khóa tài khoản." : "Đã khóa tài khoản.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var taiKhoan = _context.TaiKhoans.FirstOrDefault(t => t.MaTaiKhoan == id);
            if (taiKhoan == null) return NotFound();

            ViewBag.CoHoSoSinhVien = _context.SinhViens.Any(s => s.MaTaiKhoan == id);
            return View(taiKhoan);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var taiKhoan = _context.TaiKhoans.FirstOrDefault(t => t.MaTaiKhoan == id);
            if (taiKhoan == null) return NotFound();

            // CHẶN CỨNG: không xóa tài khoản đã có hồ sơ Sinh viên liên kết (quan hệ 1-1 bắt buộc
            // do Module 3 quản lý) - xóa sẽ cascade xóa luôn hồ sơ SinhVien và có thể vướng FK
            // Restrict với DangKyDeTai, phá dữ liệu chung. Dùng "Khóa tài khoản" thay thế.
            if (_context.SinhViens.Any(s => s.MaTaiKhoan == id))
            {
                TempData["Error"] = "Không thể xóa: tài khoản đã gắn hồ sơ Sinh viên. " +
                                     "Vui lòng dùng chức năng Khóa tài khoản thay vì xóa.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.TaiKhoans.Remove(taiKhoan);
                _context.SaveChanges();
                TempData["Success"] = "Đã xóa tài khoản.";
            }
            catch (DbUpdateException)
            {
                // Lưới an toàn cuối cùng nếu còn ràng buộc khác chưa lường trước được
                TempData["Error"] = "Không thể xóa tài khoản này vì đang được dữ liệu khác tham chiếu.";
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult DoiMatKhau(int id)
        {
            var taiKhoan = _context.TaiKhoans.FirstOrDefault(t => t.MaTaiKhoan == id);
            if (taiKhoan == null) return NotFound();

            return View(new DoiMatKhauViewModel { MaTaiKhoan = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DoiMatKhau(DoiMatKhauViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var taiKhoan = _context.TaiKhoans.FirstOrDefault(t => t.MaTaiKhoan == model.MaTaiKhoan);
            if (taiKhoan == null) return NotFound();

            taiKhoan.MatKhau = model.MatKhauMoi;
            _context.SaveChanges();

            TempData["Success"] = "Đặt lại mật khẩu thành công.";
            return RedirectToAction(nameof(Index));
        }
    }
}
