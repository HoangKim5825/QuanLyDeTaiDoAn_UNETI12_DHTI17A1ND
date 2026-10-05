// Ghi chú: Controller này thuộc phạm vi phụ trách chính của Module 1
// (Đăng nhập - Đăng xuất - Phân quyền). Đây là bản TỐI THIỂU chỉ đủ để
// Module 3 có thể đăng nhập và chạy thử độc lập trong lúc chờ tích hợp
// với phần đầy đủ do bạn phụ trách Module 1 xây dựng (băm mật khẩu,
// khóa tài khoản sai nhiều lần, ASP.NET Core Identity nếu nhóm chọn làm nâng cao...).

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyDeTaiDoAn.Common;
using QuanLyDeTaiDoAn.Data;

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
        public IActionResult Login()
        {
            // Nếu đã đăng nhập rồi thì về trang chủ luôn
            if (HttpContext.Session.GetInt32("MaTaiKhoan") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string tenDangNhap, string matKhau)
        {
            if (string.IsNullOrWhiteSpace(tenDangNhap) || string.IsNullOrWhiteSpace(matKhau))
            {
                ViewBag.Loi = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.";
                return View();
            }

            // Dùng EF Core + LINQ để kiểm tra tài khoản (đúng yêu cầu mục 5.2)
            var taiKhoan = _context.TaiKhoans
                .FirstOrDefault(tk => tk.TenDangNhap == tenDangNhap && tk.MatKhau == matKhau);

            if (taiKhoan == null)
            {
                ViewBag.Loi = "Tên đăng nhập hoặc mật khẩu không đúng.";
                return View();
            }

            if (taiKhoan.TrangThai == AppConstants.TaiKhoan_Khoa)
            {
                ViewBag.Loi = "Tài khoản đã bị khóa, vui lòng liên hệ quản trị viên.";
                return View();
            }

            // Đăng nhập thành công -> lưu tối thiểu vào Session (đúng mục 5.2)
            HttpContext.Session.SetInt32("MaTaiKhoan", taiKhoan.MaTaiKhoan);
            HttpContext.Session.SetString("HoTen", taiKhoan.HoTen);
            HttpContext.Session.SetString("VaiTro", taiKhoan.VaiTro);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            // Xóa toàn bộ thông tin đăng nhập khỏi Session (đúng mục 5.2)
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
