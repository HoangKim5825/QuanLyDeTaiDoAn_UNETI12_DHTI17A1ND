// Họ và tên: Nguyễn Minh Đức
// Mã sinh viên: [23203100078]
// Nội dung thực hiện: Module 1 - ViewModel cho đăng nhập và quản lý tài khoản


using Microsoft.AspNetCore.Mvc;
using QuanLyDeTaiDoAn.Data;
using QuanLyDeTaiDoAn.Filters;
using QuanLyDeTaiDoAn.ViewModels;

namespace QuanLyDeTaiDoAn.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (HttpContext.Session.GetInt32(YeuCauDangNhapAttribute.SessionMaTaiKhoan) != null)
                return RedirectToAction("Index", "Home");

            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Dùng EF Core + LINQ để kiểm tra tài khoản trên bảng TaiKhoan dùng chung của nhóm
            var taiKhoan = _context.TaiKhoans
                .FirstOrDefault(t => t.TenDangNhap == model.TenDangNhap);

            if (taiKhoan == null)
            {
                ModelState.AddModelError(string.Empty, "Tên đăng nhập không tồn tại.");
                return View(model);
            }

            if (!taiKhoan.TrangThai)
            {
                ModelState.AddModelError(string.Empty, "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên.");
                return View(model);
            }

            if (taiKhoan.MatKhau != model.MatKhau)
            {
                ModelState.AddModelError(string.Empty, "Mật khẩu không đúng.");
                return View(model);
            }

            HttpContext.Session.SetInt32(YeuCauDangNhapAttribute.SessionMaTaiKhoan, taiKhoan.MaTaiKhoan);
            HttpContext.Session.SetString(YeuCauDangNhapAttribute.SessionHoTen, taiKhoan.HoTen);
            HttpContext.Session.SetString(YeuCauDangNhapAttribute.SessionVaiTro, taiKhoan.VaiTro);

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                return Redirect(model.ReturnUrl);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
