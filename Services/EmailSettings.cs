// Họ và tên: Hoàng Kim
// Mã sinh viên: 23203100056
// Nội dung thực hiện: Module 4 - Cấu hình gửi email (SMTP Gmail)
namespace QuanLyDeTaiDoAn.Services
{
    public class EmailSettings
    {
        public bool Enabled { get; set; } = true;
        public string Host { get; set; } = "smtp.gmail.com";
        public int Port { get; set; } = 587;
        public string Username { get; set; } = string.Empty;   // từ User Secrets
        public string Password { get; set; } = string.Empty;   // App Password, từ User Secrets
        public string FromName { get; set; } = "Quản lý đề tài";
    }
}