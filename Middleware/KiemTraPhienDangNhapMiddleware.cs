// Họ và tên: Nguyễn Minh Đức
// Mã sinh viên: 23203100078
// Nội dung thực hiện: Module 1 - Middleware kiểm tra phiên đăng nhập với CSDL ở mỗi request
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using QuanLyDeTaiDoAn.Data;
using QuanLyDeTaiDoAn.Filters;

namespace QuanLyDeTaiDoAn.Middleware
{
    // Vấn đề: Session chỉ lưu lúc đăng nhập, nên Admin khóa/xóa tài khoản hoặc đổi vai trò
    // thì người đó vẫn dùng được cho tới khi Session hết hạn (30 phút).
    // Giải pháp: với mỗi request vào Controller, đối chiếu lại tài khoản trong Session với bảng TaiKhoan:
    //   - Không còn tồn tại hoặc đã bị khóa  -> xóa Session, chuyển về trang Đăng nhập kèm lý do.
    //   - Vẫn hợp lệ                         -> làm mới Họ tên và Vai trò trong Session theo CSDL.
    // Vì chạy trước mọi Controller nên cũng bảo vệ các module khác đang đọc Session["VaiTro"].
    public class KiemTraPhienDangNhapMiddleware
    {
        private readonly RequestDelegate _next;

        public KiemTraPhienDangNhapMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ApplicationDbContext db)
        {
            // Bỏ qua file tĩnh (css/js/ảnh): chỉ kiểm tra request đi vào Controller action.
            bool laControllerAction =
                context.GetEndpoint()?.Metadata.GetMetadata<ControllerActionDescriptor>() != null;

            int? maTaiKhoan = laControllerAction
                ? context.Session.GetInt32(YeuCauDangNhapAttribute.SessionMaTaiKhoan)
                : null;

            if (maTaiKhoan != null)
            {
                var taiKhoan = await db.TaiKhoans
                    .AsNoTracking()
                    .Where(t => t.MaTaiKhoan == maTaiKhoan)
                    .Select(t => new { t.TrangThai, t.VaiTro, t.HoTen })
                    .FirstOrDefaultAsync();

                if (taiKhoan == null || !taiKhoan.TrangThai)
                {
                    context.Session.Clear();
                    string lyDo = taiKhoan == null ? "khongTonTai" : "biKhoa";
                    context.Response.Redirect("/Account/Login?lyDo=" + lyDo);
                    return;
                }

                // Đồng bộ lại thông tin có thể đã thay đổi từ khi đăng nhập
                context.Session.SetString(YeuCauDangNhapAttribute.SessionVaiTro, taiKhoan.VaiTro);
                context.Session.SetString(YeuCauDangNhapAttribute.SessionHoTen, taiKhoan.HoTen);
            }

            await _next(context);
        }
    }
}
