// Họ và tên: Nguyễn Minh Đức
// Mã sinh viên: [23203100078]
// Nội dung thực hiện: Module 1 - ViewModel cho đăng nhập và quản lý tài khoản

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using QuanLyDeTaiDoAn.Helpers;

namespace QuanLyDeTaiDoAn.Filters
{
    // Bắt buộc đã đăng nhập VÀ vai trò = Admin. Vai trò đọc từ Session do server set lúc
    // đăng nhập thành công - KHÔNG tin vai trò do Client tự gửi lên.
    public class YeuCauAdminAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var maTaiKhoan = context.HttpContext.Session.GetInt32(YeuCauDangNhapAttribute.SessionMaTaiKhoan);

            if (maTaiKhoan == null)
            {
                var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
                context.Result = new RedirectToActionResult("Login", "Account", new { returnUrl });
                return;
            }

            var vaiTro = context.HttpContext.Session.GetString(YeuCauDangNhapAttribute.SessionVaiTro);

            if (vaiTro != VaiTroTaiKhoan.Admin)
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
