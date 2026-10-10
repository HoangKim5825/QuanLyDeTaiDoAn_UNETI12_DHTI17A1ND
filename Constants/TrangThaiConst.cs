// Họ và tên: <Nguyen Quang Huy>
// Mã sinh viên: <23203100080>
// Nội dung thực hiện: Hằng số trạng thái dùng chung (Module 2)
namespace QuanLyDeTaiDoAn.Constants
{
    // Giá trị phải khớp 100% với SeedData.cs. Không gõ chuỗi trực tiếp trong LINQ.
    public static class TrangThaiConst
    {
        public static class DeTaiTT
        {
            public const string MoDangKy = "MoDangKy";
            public const string DuSoLuong = "DuSoLuong";   // chỉ để hiển thị/lọc, tính từ SoChoCon
            public const string TamKhoa = "TamKhoa";
            public const string KetThuc = "KetThuc";

            public static readonly string[] TatCa = { MoDangKy, DuSoLuong, TamKhoa, KetThuc };

            public static string Nhan(string? tt) => tt switch
            {
                MoDangKy => "Mở đăng ký",
                DuSoLuong => "Đủ số lượng",
                TamKhoa => "Tạm khóa",
                KetThuc => "Kết thúc",
                _ => tt ?? ""
            };
        }

        public static class GiangVienTT
        {
            public const string DangCongTac = "DangCongTac";
            public const string TamNgungHuongDan = "TamNgungHuongDan";

            public static readonly string[] TatCa = { DangCongTac, TamNgungHuongDan };

            public static string Nhan(string? tt) => tt switch
            {
                DangCongTac => "Đang công tác",
                TamNgungHuongDan => "Tạm ngừng hướng dẫn",
                _ => tt ?? ""
            };
        }

        public static class DangKyTT
        {
            public const string ChoDuyet = "ChoDuyet";
            public const string DaDuyet = "DaDuyet";   // duy nhất trạng thái chiếm chỗ
            public const string TuChoi = "TuChoi";
            public const string DaHuy = "DaHuy";
        }

        public static class PhanCongTT
        {
            public const string DangHuongDan = "DangHuongDan";
            public const string DaThayDoi = "DaThayDoi";
            public const string KetThuc = "KetThuc";
        }
    }
}
