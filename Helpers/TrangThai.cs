// Họ và tên: Hoàng Kim
// Mã sinh viên: <23203100056>
// Nội dung thực hiện: Module 4 - Hằng số trạng thái dùng chung (tránh gõ sai chuỗi)
// Lưu ý: giá trị khớp đúng với chuỗi đang lưu trong DB / SeedData.

namespace QuanLyDeTaiDoAn.Helpers
{
    public static class TrangThaiDangKy
    {
        public const string ChoDuyet = "ChoDuyet";
        public const string DaDuyet = "DaDuyet";
        public const string TuChoi = "TuChoi";
        public const string DaHuy = "DaHuy";
    }

    public static class TrangThaiPhanCong
    {
        public const string DangHuongDan = "DangHuongDan";
        public const string DaThayDoi = "DaThayDoi";
        public const string KetThuc = "KetThuc";
    }

    public static class TrangThaiGiangVien
    {
        public const string DangCongTac = "DangCongTac";
        public const string TamNgungHuongDan = "TamNgungHuongDan";
    }

    public static class TrangThaiDeTai
    {
        public const string MoDangKy = "MoDangKy";
        public const string DuSoLuong = "DuSoLuong";
        public const string TamKhoa = "TamKhoa";
        public const string KetThuc = "KetThuc";
    }
    public static class TienDoThucHien
    {
        public const string ChuaBatDau = "ChuaBatDau";
        public const string DangThucHien = "DangThucHien";
        public const string HoanThanh = "HoanThanh";
        public static readonly string[] ThuTu = { ChuaBatDau, DangThucHien, HoanThanh };
    }
}