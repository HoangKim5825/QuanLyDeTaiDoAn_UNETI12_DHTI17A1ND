using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace QuanLyDeTaiDoAn.Filters
{
    /// <summary>
    /// Chặn truy cập nếu vai trò trong Session khác với vai trò yêu cầu.
    /// Đáp ứng đúng yêu cầu đề bài (mục 5.3): "Các chức năng dành cho Admin
    /// phải kiểm tra quyền tại Controller trước khi xử lý, không chỉ ẩn nút
    /// hoặc menu trên View." Vai trò LUÔN lấy từ Session (do đăng nhập ghi vào),
    /// KHÔNG bao giờ lấy giá trị do người dùng tự gửi lên từ Form/URL.
    ///
    /// Cách dùng:
    /// [RequireLogin]
    /// [RequireRole("Admin")]
    /// public IActionResult Index() { ... }
    /// </summary>
    public class RequireRoleAttribute : ActionFilterAttribute
    {
        private readonly string _vaiTroYeuCau;

        public RequireRoleAttribute(string vaiTroYeuCau)
        {
            _vaiTroYeuCau = vaiTroYeuCau;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var vaiTro = context.HttpContext.Session.GetString("VaiTro");

            if (string.IsNullOrEmpty(vaiTro))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            if (!string.Equals(vaiTro, _vaiTroYeuCau, StringComparison.OrdinalIgnoreCase))
            {
                // Không đúng vai trò yêu cầu -> từ chối truy cập
                context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
