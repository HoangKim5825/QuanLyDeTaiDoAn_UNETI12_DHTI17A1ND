// Họ và tên: Nguyễn Minh Đức
// Mã sinh viên: 23203100078
// Nội dung thực hiện: Module 1 - Filter bắt buộc đăng nhập (kiểm tra Session) cho Controller/Action
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace QuanLyDeTaiDoAn.Filters
{
    // Áp dụng: [YeuCauDangNhap] trên Controller hoặc Action cần đăng nhập mới được truy cập.
    public class YeuCauDangNhapAttribute : ActionFilterAttribute
    {
        public const string SessionMaTaiKhoan = "MaTaiKhoan";
        public const string SessionHoTen = "HoTen";
        public const string SessionVaiTro = "VaiTro";

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var maTaiKhoan = context.HttpContext.Session.GetInt32(SessionMaTaiKhoan);

            if (maTaiKhoan == null)
            {
                var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
                context.Result = new RedirectToActionResult("Login", "Account", new { returnUrl });
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
