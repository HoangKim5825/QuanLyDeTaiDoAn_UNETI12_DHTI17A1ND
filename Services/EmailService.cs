// Họ và tên: Hoàng Kim
// Mã sinh viên: 23203100056
// Nội dung thực hiện: Module 4 - Gửi email thông báo kết quả duyệt / phân công / đổi GVHD (MailKit + Gmail SMTP)
using System.Net;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MimeKit;
using QuanLyDeTaiDoAn.Data;

namespace QuanLyDeTaiDoAn.Services
{
    public class EmailService
    {
        private readonly EmailSettings _cfg;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> cfg, ApplicationDbContext context, ILogger<EmailService> logger)
        {
            _cfg = cfg.Value;
            _context = context;
            _logger = logger;
        }

        // ===== Gửi 1 email. KHÔNG BAO GIỜ ném exception: lỗi chỉ ghi log, trả false =====
        public async Task<bool> GuiAsync(string? den, string tenNguoiNhan, string tieuDe, string noiDungHtml)
        {
            if (!_cfg.Enabled || string.IsNullOrWhiteSpace(_cfg.Username)
                || string.IsNullOrWhiteSpace(_cfg.Password) || string.IsNullOrWhiteSpace(den))
            {
                _logger.LogWarning("Bỏ qua gửi email '{TieuDe}' (chưa cấu hình hoặc thiếu địa chỉ nhận).", tieuDe);
                return false;
            }

            try
            {
                var msg = new MimeMessage();
                msg.From.Add(new MailboxAddress(_cfg.FromName, _cfg.Username));
                msg.To.Add(new MailboxAddress(tenNguoiNhan, den));
                msg.Subject = tieuDe;
                msg.Body = new BodyBuilder { HtmlBody = Khung(tieuDe, noiDungHtml) }.ToMessageBody();

                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_cfg.Host, _cfg.Port, SecureSocketOptions.StartTls, cts.Token);
                await smtp.AuthenticateAsync(_cfg.Username, _cfg.Password, cts.Token);
                await smtp.SendAsync(msg, cts.Token);
                await smtp.DisconnectAsync(true, cts.Token);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gửi email '{TieuDe}' tới {Den} thất bại.", tieuDe, den);
                return false;
            }
        }

        // ===== 1. Kết quả duyệt / từ chối =====
        public async Task GuiKetQuaDuyetAsync(int maDangKy, bool duyet, string? lyDo = null)
        {
            var t = await LayThongTinAsync(maDangKy);
            if (t == null) return;

            string body = duyet
                ? $"<p>Đăng ký đề tài <b>{H(t.TenDeTai)}</b> của bạn đã được <b style='color:#198754'>DUYỆT</b>.</p>" +
                  "<p>Bạn sẽ nhận thêm thông báo khi được phân công giảng viên hướng dẫn.</p>"
                : $"<p>Đăng ký đề tài <b>{H(t.TenDeTai)}</b> của bạn đã bị <b style='color:#dc3545'>TỪ CHỐI</b>.</p>" +
                  $"<p>Lý do: {H(lyDo)}</p>";

            await GuiAsync(t.EmailSv, t.TenSv,
                duyet ? "Đăng ký đề tài đã được duyệt" : "Đăng ký đề tài bị từ chối", Chao(t.TenSv) + body);
        }

        // ===== 2. Phân công GVHD: báo cho sinh viên và giảng viên =====
        public async Task GuiPhanCongAsync(int maDangKy, int maGiangVien)
        {
            var t = await LayThongTinAsync(maDangKy);
            var gv = await _context.GiangViens.AsNoTracking().FirstOrDefaultAsync(g => g.MaGiangVien == maGiangVien);
            if (t == null || gv == null) return;

            await GuiAsync(t.EmailSv, t.TenSv, "Bạn đã được phân công giảng viên hướng dẫn",
                Chao(t.TenSv) +
                $"<p>Đề tài <b>{H(t.TenDeTai)}</b> đã được phân công giảng viên hướng dẫn: " +
                $"<b>{H(gv.HocVi)} {H(gv.HoTen)}</b> (email: {H(gv.Email)}).</p>");

            await GuiAsync(gv.Email, gv.HoTen, "Bạn được phân công hướng dẫn sinh viên mới",
                Chao(gv.HoTen) +
                $"<p>Bạn được phân công hướng dẫn sinh viên <b>{H(t.TenSv)}</b> (mã SV {t.MaSv}) " +
                $"thực hiện đề tài <b>{H(t.TenDeTai)}</b>.</p>");
        }

        // ===== 3. Đổi GVHD: báo cho sinh viên, GV cũ và GV mới =====
        public async Task GuiDoiGVHDAsync(int maDangKy, int maGvCu, int maGvMoi, string lyDo)
        {
            var t = await LayThongTinAsync(maDangKy);
            var ds = await _context.GiangViens.AsNoTracking()
                .Where(g => g.MaGiangVien == maGvCu || g.MaGiangVien == maGvMoi).ToListAsync();
            var cu = ds.FirstOrDefault(g => g.MaGiangVien == maGvCu);
            var moi = ds.FirstOrDefault(g => g.MaGiangVien == maGvMoi);
            if (t == null || cu == null || moi == null) return;

            await GuiAsync(t.EmailSv, t.TenSv, "Giảng viên hướng dẫn của bạn đã thay đổi",
                Chao(t.TenSv) +
                $"<p>Giảng viên hướng dẫn đề tài <b>{H(t.TenDeTai)}</b> đã đổi từ <b>{H(cu.HoTen)}</b> " +
                $"sang <b>{H(moi.HocVi)} {H(moi.HoTen)}</b> (email: {H(moi.Email)}).</p><p>Lý do: {H(lyDo)}</p>");

            await GuiAsync(cu.Email, cu.HoTen, "Kết thúc hướng dẫn một sinh viên",
                Chao(cu.HoTen) +
                $"<p>Bạn không còn hướng dẫn sinh viên <b>{H(t.TenSv)}</b> (đề tài {H(t.TenDeTai)}). Lý do: {H(lyDo)}</p>");

            await GuiAsync(moi.Email, moi.HoTen, "Bạn được phân công hướng dẫn sinh viên mới",
                Chao(moi.HoTen) +
                $"<p>Bạn được phân công hướng dẫn sinh viên <b>{H(t.TenSv)}</b> (mã SV {t.MaSv}), " +
                $"đề tài <b>{H(t.TenDeTai)}</b>.</p>");
        }

        // ===== Hàm hỗ trợ =====
        private record ThongTin(int MaSv, string TenSv, string? EmailSv, string TenDeTai);

        private Task<ThongTin?> LayThongTinAsync(int maDangKy) =>
            _context.DangKyDeTais.AsNoTracking()
                .Where(dk => dk.MaDangKy == maDangKy)
                .Select(dk => new ThongTin(dk.MaSinhVien, dk.SinhVien!.HoTen, dk.SinhVien.Email, dk.DeTai!.TenDeTai))
                .FirstOrDefaultAsync();

        // Mọi chuỗi do người dùng nhập đều phải encode trước khi đưa vào HTML
        private static string H(string? s) => WebUtility.HtmlEncode(s ?? "");
        private static string Chao(string ten) => $"<p>Xin chào <b>{H(ten)}</b>,</p>";

        private static string Khung(string tieuDe, string noiDung) =>
            "<div style='font-family:Arial,sans-serif;max-width:600px;margin:auto;border:1px solid #dee2e6'>" +
            $"<div style='background:#0d6efd;color:#fff;padding:12px 16px'><b>{H(tieuDe)}</b></div>" +
            $"<div style='padding:16px'>{noiDung}</div>" +
            "<div style='background:#f8f9fa;color:#6c757d;padding:8px 16px;font-size:12px'>" +
            "Email tự động từ Hệ thống quản lý đề tài đồ án. Vui lòng không trả lời.</div></div>";
    }
}