using Microsoft.EntityFrameworkCore;
using QuanLyDeTaiDoAn.Common;
using QuanLyDeTaiDoAn.Models;

namespace QuanLyDeTaiDoAn.Data
{
    /// <summary>
    /// Dữ liệu mẫu tối thiểu để CHẠY THỬ Module 3 (đăng nhập, đăng ký, hủy đăng ký,
    /// theo dõi kết quả). Đây CHƯA phải bộ dữ liệu đầy đủ theo mục 15 của đề bài
    /// (05 lĩnh vực / 12 giảng viên / 20 đề tài / 30 sinh viên) - cả nhóm cần bổ
    /// sung thêm dữ liệu (đặc biệt là DeTai) khi tích hợp bản nộp cuối cùng để có
    /// đủ số trang khi kiểm thử Phân trang (mục 15.1).
    /// </summary>
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext db)
        {
            db.Database.EnsureCreated(); // Dùng tạm khi chưa chạy Migration; nếu đã có Migration hãy dùng db.Database.Migrate() thay cho dòng này.

            if (db.TaiKhoans.Any()) return; // Đã có dữ liệu thì không seed lại

            // ---------- 1. Lĩnh vực ----------
            var lv1 = new LinhVuc { TenLinhVuc = "Công nghệ phần mềm", MoTa = "Phát triển ứng dụng web/app", TrangThai = "Hoạt động" };
            var lv2 = new LinhVuc { TenLinhVuc = "Trí tuệ nhân tạo", MoTa = "AI, Machine Learning", TrangThai = "Hoạt động" };
            var lv3 = new LinhVuc { TenLinhVuc = "Mạng máy tính", MoTa = "Hệ thống mạng, bảo mật", TrangThai = "Hoạt động" };
            db.LinhVucs.AddRange(lv1, lv2, lv3);
            db.SaveChanges();

            // ---------- 2. Giảng viên ----------
            var gv1 = new GiangVien { HoTen = "Nguyễn Văn An", HocVi = "Thạc sĩ", Email = "an.nv@uneti.edu.vn", SoDienThoai = "0901111111", MaLinhVuc = lv1.MaLinhVuc, SoSinhVienHuongDanToiDa = 3, TrangThai = AppConstants.GiangVien_DangCongTac };
            var gv2 = new GiangVien { HoTen = "Trần Thị Bình", HocVi = "Tiến sĩ", Email = "binh.tt@uneti.edu.vn", SoDienThoai = "0902222222", MaLinhVuc = lv2.MaLinhVuc, SoSinhVienHuongDanToiDa = 2, TrangThai = AppConstants.GiangVien_DangCongTac };
            var gv3 = new GiangVien { HoTen = "Lê Văn Cường", HocVi = "Thạc sĩ", Email = "cuong.lv@uneti.edu.vn", SoDienThoai = "0903333333", MaLinhVuc = lv3.MaLinhVuc, SoSinhVienHuongDanToiDa = 4, TrangThai = AppConstants.GiangVien_DangCongTac };
            db.GiangViens.AddRange(gv1, gv2, gv3);
            db.SaveChanges();

            // ---------- 3. Đề tài ----------
            string namHoc = AppConstants.NamHocHienTai;
            string hocKy = AppConstants.HocKyHienTai;

            var dt1 = new DeTai { TenDeTai = "Xây dựng Website bán hàng ASP.NET Core", MaLinhVuc = lv1.MaLinhVuc, MoTa = "Website TMĐT cơ bản", YeuCau = "Nắm vững MVC, EF Core", SoSinhVienToiDa = 3, NamHoc = namHoc, HocKy = hocKy, TrangThai = AppConstants.DeTai_MoDangKy };
            var dt2 = new DeTai { TenDeTai = "Hệ thống quản lý đề tài đồ án", MaLinhVuc = lv1.MaLinhVuc, MoTa = "Chính là đề tài 12 này :)", YeuCau = "MVC, EF Core, LINQ", SoSinhVienToiDa = 1, NamHoc = namHoc, HocKy = hocKy, TrangThai = AppConstants.DeTai_MoDangKy };
            var dt3 = new DeTai { TenDeTai = "Ứng dụng nhận diện ảnh bằng AI", MaLinhVuc = lv2.MaLinhVuc, MoTa = "Image classification", YeuCau = "Python/ML cơ bản", SoSinhVienToiDa = 2, NamHoc = namHoc, HocKy = hocKy, TrangThai = AppConstants.DeTai_MoDangKy };
            var dt4 = new DeTai { TenDeTai = "Hệ thống giám sát mạng nội bộ", MaLinhVuc = lv3.MaLinhVuc, MoTa = "Network monitoring", YeuCau = "Kiến thức mạng cơ bản", SoSinhVienToiDa = 2, NamHoc = namHoc, HocKy = hocKy, TrangThai = AppConstants.DeTai_MoDangKy };
            var dt5 = new DeTai { TenDeTai = "Chatbot hỗ trợ sinh viên", MaLinhVuc = lv2.MaLinhVuc, MoTa = "Chatbot tư vấn", YeuCau = "NLP cơ bản", SoSinhVienToiDa = 2, NamHoc = namHoc, HocKy = hocKy, TrangThai = AppConstants.DeTai_TamKhoa };
            db.DeTais.AddRange(dt1, dt2, dt3, dt4, dt5);
            db.SaveChanges();

            // ---------- 4. Tài khoản Admin ----------
            var admin = new TaiKhoan { TenDangNhap = "admin", MatKhau = "123456", HoTen = "Quản trị hệ thống", Email = "admin@uneti.edu.vn", VaiTro = AppConstants.VaiTro_Admin, TrangThai = AppConstants.TaiKhoan_HoatDong };
            db.TaiKhoans.Add(admin);
            db.SaveChanges();

            // ---------- 5. Tài khoản + Sinh viên ----------
            (string tk, string mssv, string hoTen, string lop, string trangThai) [] dsSv = new[]
            {
                ("sv001", "22103100001", "Phạm Thị Dung", "TI17A1HN", AppConstants.SinhVien_DangHoc),
                ("sv002", "22103100002", "Hoàng Văn Em",  "TI17A1HN", AppConstants.SinhVien_DangHoc),
                ("sv003", "22103100003", "Vũ Thị Giang",  "TI17A1HN", AppConstants.SinhVien_DangHoc),
                ("sv004", "22103100004", "Đặng Văn Huy",  "TI17A2HN", AppConstants.SinhVien_DangHoc),
                ("sv005", "22103100005", "Bùi Thị Kim",   "TI17A2HN", AppConstants.SinhVien_DangHoc),
                ("sv006", "22103100006", "Ngô Văn Long",  "TI17A2HN", AppConstants.SinhVien_NgungHoc),
            };

            var sinhViens = new List<SinhVien>();
            foreach (var s in dsSv)
            {
                var tk = new TaiKhoan
                {
                    TenDangNhap = s.tk,
                    MatKhau = "123456",
                    HoTen = s.hoTen,
                    Email = s.mssv + "@sv.uneti.edu.vn",
                    VaiTro = AppConstants.VaiTro_SinhVien,
                    TrangThai = AppConstants.TaiKhoan_HoatDong
                };
                db.TaiKhoans.Add(tk);
                db.SaveChanges(); // Lưu để có MaTaiKhoan trước khi tạo SinhVien

                var sv = new SinhVien
                {
                    MaSinhVien = s.mssv,
                    MaTaiKhoan = tk.MaTaiKhoan,
                    HoTen = s.hoTen,
                    NgaySinh = new DateTime(2004, 1, 1),
                    Lop = s.lop,
                    Khoa = "Công nghệ thông tin",
                    ChuyenNganh = "Kỹ thuật phần mềm",
                    Email = tk.Email,
                    SoDienThoai = "09" + s.mssv.Substring(s.mssv.Length - 8),
                    TrangThai = s.trangThai
                };
                sinhViens.Add(sv);
            }
            db.SinhViens.AddRange(sinhViens);
            db.SaveChanges();

            // ---------- 6. Đăng ký đề tài (đủ các trạng thái để kiểm thử) ----------
            var dk1 = new DangKyDeTai { MaSinhVien = sinhViens[0].MaSinhVien, MaDeTai = dt1.MaDeTai, NgayDangKy = DateTime.Now.AddDays(-10), LyDoDangKy = "Yêu thích lập trình web", TrangThai = AppConstants.DangKy_DaDuyet, GhiChuDuyet = "Đủ điều kiện" };
            var dk2 = new DangKyDeTai { MaSinhVien = sinhViens[1].MaSinhVien, MaDeTai = dt1.MaDeTai, NgayDangKy = DateTime.Now.AddDays(-9), LyDoDangKy = "Muốn tìm hiểu ASP.NET Core", TrangThai = AppConstants.DangKy_DaDuyet, GhiChuDuyet = "Đủ điều kiện" };
            var dk3 = new DangKyDeTai { MaSinhVien = sinhViens[2].MaSinhVien, MaDeTai = dt3.MaDeTai, NgayDangKy = DateTime.Now.AddDays(-5), LyDoDangKy = "Quan tâm AI", TrangThai = AppConstants.DangKy_ChoDuyet, GhiChuDuyet = null };
            var dk4 = new DangKyDeTai { MaSinhVien = sinhViens[3].MaSinhVien, MaDeTai = dt4.MaDeTai, NgayDangKy = DateTime.Now.AddDays(-3), LyDoDangKy = "Thích mạng máy tính", TrangThai = AppConstants.DangKy_ChoDuyet, GhiChuDuyet = null };
            var dk5 = new DangKyDeTai { MaSinhVien = sinhViens[4].MaSinhVien, MaDeTai = dt3.MaDeTai, NgayDangKy = DateTime.Now.AddDays(-20), LyDoDangKy = "Đăng ký thử rồi đổi ý", TrangThai = AppConstants.DangKy_DaHuy, GhiChuDuyet = null };
            db.DangKyDeTais.AddRange(dk1, dk2, dk3, dk4, dk5);
            db.SaveChanges();

            // ---------- 7. Phân công hướng dẫn (cho các đăng ký đã duyệt) ----------
            var pc1 = new PhanCongHuongDan { MaDangKy = dk1.MaDangKy, MaGiangVien = gv1.MaGiangVien, NgayBatDau = DateTime.Now.AddDays(-8), NgayKetThuc = null, TrangThai = AppConstants.PhanCong_DangHuongDan, GhiChu = "Phân công lần đầu" };
            var pc2 = new PhanCongHuongDan { MaDangKy = dk2.MaDangKy, MaGiangVien = gv1.MaGiangVien, NgayBatDau = DateTime.Now.AddDays(-7), NgayKetThuc = null, TrangThai = AppConstants.PhanCong_DangHuongDan, GhiChu = "Phân công lần đầu" };
            db.PhanCongHuongDans.AddRange(pc1, pc2);
            db.SaveChanges();
        }
    }
}
