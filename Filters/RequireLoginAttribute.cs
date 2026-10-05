using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace QuanLyDeTaiDoAn.Filters
{
    /// <summary>
    /// Chặn truy cập nếu người dùng chưa đăng nhập (chưa có MaTaiKhoan trong Session).
    /// Áp dụng cho toàn bộ Controller/Action cần đăng nhập, ví dụ:
    /// [RequireLogin]
    /// public class SinhVienController : Controller { ... }
    ///
    /// Đây là ví dụ tối thiểu để Module 3 chạy độc lập được; nhóm nên thống nhất
    /// với bạn phụ trách Module 1 để dùng chung 1 cơ chế kiểm tra đăng nhập/
    /// phân quyền duy nhất cho toàn bộ Project (tránh mỗi Module viết 1 kiểu).
    /// </summary>
    public class RequireLoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var maTaiKhoan = context.HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan == null)
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
