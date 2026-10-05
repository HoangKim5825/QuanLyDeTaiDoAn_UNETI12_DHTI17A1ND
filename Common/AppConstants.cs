namespace QuanLyDeTaiDoAn.Common
{
    /// <summary>
    /// Các hằng số dùng chung cho TOÀN BỘ Project (cả 4 Module).
    /// Mục đích: tránh mỗi Module tự gõ chuỗi trạng thái khác nhau
    /// (vd người viết "Đã duyệt", người viết "Da duyet") gây sai lệch dữ liệu.
    /// Cả nhóm nên thống nhất dùng các hằng số này khi so sánh/gán TrangThai.
    /// </summary>
    public static class AppConstants
    {
        // ---- Vai trò tài khoản (Module 1) ----
        public const string VaiTro_Admin = "Admin";
        public const string VaiTro_SinhVien = "SinhVien";

        // ---- Trạng thái Tài khoản (Module 1) ----
        public const string TaiKhoan_HoatDong = "Hoạt động";
        public const string TaiKhoan_Khoa = "Khóa";

        // ---- Trạng thái Sinh viên (Module 3) ----
        public const string SinhVien_DangHoc = "Đang học";
        public const string SinhVien_NgungHoc = "Ngừng học";

        // ---- Trạng thái Giảng viên (Module 2) ----
        public const string GiangVien_DangCongTac = "Đang công tác";
        public const string GiangVien_TamNgungHuongDan = "Tạm ngừng hướng dẫn";

        // ---- Trạng thái Đề tài (Module 2) ----
        public const string DeTai_MoDangKy = "Mở đăng ký";
        public const string DeTai_DuSoLuong = "Đủ số lượng";
        public const string DeTai_TamKhoa = "Tạm khóa";
        public const string DeTai_KetThuc = "Kết thúc";

        // ---- Trạng thái Đăng ký đề tài (Module 3) ----
        public const string DangKy_ChoDuyet = "Chờ duyệt";
        public const string DangKy_DaDuyet = "Đã duyệt";
        public const string DangKy_TuChoi = "Từ chối";
        public const string DangKy_DaHuy = "Đã hủy";

        /// <summary>
        /// Các trạng thái đăng ký được TÍNH LÀ ĐANG CHIẾM CHỖ của đề tài
        /// (dùng để tính SoChoCon = SoSinhVienToiDa - SoSinhVienDangKyHopLe).
        /// Theo đề bài: chỉ "Đã duyệt" (và "đang thực hiện") mới tính, còn
        /// "Từ chối"/"Đã hủy" thì KHÔNG được tính vào số lượng đang chiếm chỗ.
        /// </summary>
        public static readonly string[] DangKy_TrangThaiHopLeDeTinhCho =
        {
            DangKy_DaDuyet
        };

        // ---- Trạng thái Phân công hướng dẫn (Module 4) ----
        public const string PhanCong_DangHuongDan = "Đang hướng dẫn";
        public const string PhanCong_DaThayDoi = "Đã thay đổi";
        public const string PhanCong_KetThuc = "Kết thúc";

        // ---- Năm học / Học kỳ đang áp dụng cho việc đăng ký đề tài ----
        // Ghi chú: nhóm có thể đổi 2 giá trị này (hoặc chuyển vào appsettings.json)
        // để quy định "đợt đăng ký hiện tại" mà sinh viên được phép đăng ký.
        public const string NamHocHienTai = "2025-2026";
        public const string HocKyHienTai = "1";
    }
}
