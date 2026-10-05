using Microsoft.AspNetCore.Mvc;

namespace QuanLyDeTaiDoAn.Controllers
{
    // Trang chủ - chỉ hiển thị thông tin chào mừng + điều hướng theo vai trò.
    // Dashboard/Thống kê đầy đủ thuộc phạm vi Module 4.
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
