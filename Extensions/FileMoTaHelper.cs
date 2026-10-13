// Họ và tên: <ĐIỀN TÊN>
// Mã sinh viên: <ĐIỀN MÃ SV>
// Nội dung thực hiện: Chức năng nâng cao - upload file mô tả đề tài, kiểm tra và lưu file an toàn (Module 2)
namespace QuanLyDeTaiDoAn.Extensions
{
    public static class FileMoTaHelper
    {
        public const long DungLuongToiDa = 5 * 1024 * 1024;   // 5 MB

        // Chỉ chấp nhận các đuôi trong danh sách này (whitelist)
        private static readonly Dictionary<string, string> LoaiFile = new(StringComparer.OrdinalIgnoreCase)
        {
            [".pdf"] = "application/pdf",
            [".doc"] = "application/msword",
            [".docx"] = "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
        };

        // Chữ ký (magic bytes) đầu file, để không tin hoàn toàn vào đuôi file do người dùng gửi
        private static readonly Dictionary<string, byte[]> ChuKy = new(StringComparer.OrdinalIgnoreCase)
        {
            [".pdf"] = new byte[] { 0x25, 0x50, 0x44, 0x46 },            // %PDF
            [".docx"] = new byte[] { 0x50, 0x4B, 0x03, 0x04 },           // ZIP (docx là file ZIP)
            [".doc"] = new byte[] { 0xD0, 0xCF, 0x11, 0xE0 }             // OLE2
        };

        // File lưu NGOÀI wwwroot nên không ai truy cập trực tiếp bằng URL được, phải qua action TaiFile
        public static string ThuMuc(IWebHostEnvironment env) =>
            Path.Combine(env.ContentRootPath, "UploadedFiles", "DeTai");

        public static string DuongDan(IWebHostEnvironment env, string tenLuu) =>
            Path.Combine(ThuMuc(env), Path.GetFileName(tenLuu));   // GetFileName chặn ../ (path traversal)

        public static string LoaiNoiDung(string tenLuu) =>
            LoaiFile.TryGetValue(Path.GetExtension(tenLuu), out var loai) ? loai : "application/octet-stream";

        // Tên gốc để hiển thị cho người dùng (chỉ giữ phần tên, bỏ đường dẫn, giới hạn độ dài)
        public static string TenHienThi(string tenGoc)
        {
            var ten = Path.GetFileName(tenGoc);
            if (ten.Length <= 255) return ten;
            var duoi = Path.GetExtension(ten);
            return ten.Substring(0, 255 - duoi.Length) + duoi;
        }

        // Trả về thông báo lỗi, hoặc null nếu file hợp lệ
        public static async Task<string?> KiemTraAsync(IFormFile file)
        {
            if (file.Length == 0) return "File rỗng.";
            if (file.Length > DungLuongToiDa) return "File vượt quá 5 MB.";

            var duoi = Path.GetExtension(file.FileName);
            if (string.IsNullOrEmpty(duoi) || !LoaiFile.ContainsKey(duoi))
                return "Chỉ chấp nhận file .pdf, .doc hoặc .docx.";

            var dau = new byte[4];
            await using var stream = file.OpenReadStream();
            int doc = await stream.ReadAtLeastAsync(dau, dau.Length, throwOnEndOfStream: false);
            if (doc < dau.Length || !dau.AsSpan().SequenceEqual(ChuKy[duoi]))
                return $"Nội dung file không đúng định dạng {duoi.ToLowerInvariant()}.";

            return null;
        }

        // Lưu file với TÊN NGẪU NHIÊN (không dùng tên gốc để tránh trùng, ghi đè, ký tự lạ). Trả về tên đã lưu.
        public static async Task<string> LuuAsync(IFormFile file, IWebHostEnvironment env)
        {
            var duoi = Path.GetExtension(file.FileName).ToLowerInvariant();
            var tenLuu = Guid.NewGuid().ToString("N") + duoi;

            Directory.CreateDirectory(ThuMuc(env));
            await using var fs = new FileStream(DuongDan(env, tenLuu), FileMode.CreateNew);
            await file.CopyToAsync(fs);
            return tenLuu;
        }

        public static void Xoa(IWebHostEnvironment env, string? tenLuu)
        {
            if (string.IsNullOrEmpty(tenLuu)) return;
            try
            {
                var path = DuongDan(env, tenLuu);
                if (File.Exists(path)) File.Delete(path);
            }
            catch (IOException) { /* file đang bị khóa: bỏ qua, không làm hỏng thao tác chính */ }
        }
    }
}
