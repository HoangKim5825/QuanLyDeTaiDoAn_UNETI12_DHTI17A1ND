using QuanLyDeTaiDoAn.Models;

namespace QuanLyDeTaiDoAn.Data
{
    public static class SeedData
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Nếu đã có dữ liệu thì không seed lại (tránh trùng khi chạy nhiều lần)
            if (context.TaiKhoans.Any()) return;

            // ===== 1. LĨNH VỰC (5) =====
            var linhVucs = new List<LinhVuc>
            {
                new() { TenLinhVuc = "Công nghệ phần mềm", MoTa = "Phát triển ứng dụng, quy trình phần mềm", TrangThai = true },
                new() { TenLinhVuc = "Trí tuệ nhân tạo & Học máy", MoTa = "AI, Machine Learning, Deep Learning", TrangThai = true },
                new() { TenLinhVuc = "Mạng và An toàn thông tin", MoTa = "Bảo mật hệ thống, an ninh mạng", TrangThai = true },
                new() { TenLinhVuc = "Hệ thống thông tin", MoTa = "Quản trị dữ liệu, hệ thống doanh nghiệp", TrangThai = true },
                new() { TenLinhVuc = "Khoa học dữ liệu", MoTa = "Phân tích và xử lý dữ liệu lớn", TrangThai = true },
            };
            context.LinhVucs.AddRange(linhVucs);
            context.SaveChanges();

            // ===== 2. GIẢNG VIÊN (12) =====
            var giangViens = new List<GiangVien>
            {
                new() { HoTen = "Nguyễn Văn An",    HocVi = "Tiến sĩ",   Email = "an.nv@uneti.edu.vn",    SoDienThoai = "0901000001", LinhVuc = linhVucs[0], SoSinhVienHuongDanToiDa = 4, TrangThai = "DangCongTac" },
                new() { HoTen = "Trần Thị Bình",    HocVi = "Thạc sĩ",   Email = "binh.tt@uneti.edu.vn",  SoDienThoai = "0901000002", LinhVuc = linhVucs[0], SoSinhVienHuongDanToiDa = 3, TrangThai = "DangCongTac" },
                new() { HoTen = "Lê Văn Cường",     HocVi = "Thạc sĩ",   Email = "cuong.lv@uneti.edu.vn", SoDienThoai = "0901000003", LinhVuc = linhVucs[0], SoSinhVienHuongDanToiDa = 2, TrangThai = "DangCongTac" },
                new() { HoTen = "Phạm Thị Dung",    HocVi = "Tiến sĩ",   Email = "dung.pt@uneti.edu.vn",  SoDienThoai = "0901000004", LinhVuc = linhVucs[1], SoSinhVienHuongDanToiDa = 3, TrangThai = "DangCongTac" },
                new() { HoTen = "Hoàng Văn Em",     HocVi = "Thạc sĩ",   Email = "em.hv@uneti.edu.vn",    SoDienThoai = "0901000005", LinhVuc = linhVucs[1], SoSinhVienHuongDanToiDa = 2, TrangThai = "DangCongTac" },
                new() { HoTen = "Vũ Thị Giang",     HocVi = "Tiến sĩ",   Email = "giang.vt@uneti.edu.vn", SoDienThoai = "0901000006", LinhVuc = linhVucs[2], SoSinhVienHuongDanToiDa = 3, TrangThai = "DangCongTac" },
                new() { HoTen = "Đặng Văn Hùng",    HocVi = "Thạc sĩ",   Email = "hung.dv@uneti.edu.vn",  SoDienThoai = "0901000007", LinhVuc = linhVucs[2], SoSinhVienHuongDanToiDa = 3, TrangThai = "DangCongTac" },
                new() { HoTen = "Bùi Thị Lan",      HocVi = "Tiến sĩ",   Email = "lan.bt@uneti.edu.vn",   SoDienThoai = "0901000008", LinhVuc = linhVucs[3], SoSinhVienHuongDanToiDa = 4, TrangThai = "DangCongTac" },
                new() { HoTen = "Ngô Văn Minh",     HocVi = "Thạc sĩ",   Email = "minh.nv@uneti.edu.vn",  SoDienThoai = "0901000009", LinhVuc = linhVucs[3], SoSinhVienHuongDanToiDa = 2, TrangThai = "TamNgungHuongDan" },
                new() { HoTen = "Đỗ Thị Nga",       HocVi = "Tiến sĩ",   Email = "nga.dt@uneti.edu.vn",   SoDienThoai = "0901000010", LinhVuc = linhVucs[4], SoSinhVienHuongDanToiDa = 3, TrangThai = "DangCongTac" },
                new() { HoTen = "Lý Văn Phúc",      HocVi = "Thạc sĩ",   Email = "phuc.lv@uneti.edu.vn",  SoDienThoai = "0901000011", LinhVuc = linhVucs[4], SoSinhVienHuongDanToiDa = 2, TrangThai = "DangCongTac" },
                new() { HoTen = "Trịnh Thị Quyên",  HocVi = "Tiến sĩ",   Email = "quyen.tt@uneti.edu.vn", SoDienThoai = "0901000012", LinhVuc = linhVucs[4], SoSinhVienHuongDanToiDa = 3, TrangThai = "DangCongTac" },
            };
            context.GiangViens.AddRange(giangViens);
            context.SaveChanges();

            // ===== 3. ĐỀ TÀI (20) =====
            string[] hocKyList = { "HK1", "HK2" };
            var deTais = new List<DeTai>();
            string[] tenDeTaiMau =
            {
                "Xây dựng hệ thống quản lý bán hàng", "Ứng dụng nhận diện khuôn mặt",
                "Hệ thống phát hiện xâm nhập mạng", "Quản lý kho vận thông minh",
                "Phân tích dữ liệu bán lẻ", "Chatbot hỗ trợ khách hàng",
                "Hệ thống đặt lịch khám bệnh", "Ứng dụng học tiếng Anh trực tuyến",
                "Quản lý thư viện điện tử", "Dự đoán giá cổ phiếu bằng Machine Learning",
                "Hệ thống chấm công nhân sự", "Website thương mại điện tử",
                "Ứng dụng quản lý chi tiêu cá nhân", "Hệ thống giám sát an ninh camera",
                "Nền tảng học trực tuyến E-learning", "Hệ thống quản lý nhà hàng",
                "Ứng dụng gợi ý phim theo sở thích", "Hệ thống quản lý ký túc xá",
                "Phân tích cảm xúc mạng xã hội", "Ứng dụng đặt xe công nghệ"
            };

            for (int i = 0; i < 20; i++)
            {
                var lv = linhVucs[i % linhVucs.Count];
                int soCho = 3; // mặc định
                string trangThai = "MoDangKy";

                // Tạo các tình huống đặc biệt theo yêu cầu 15.1
                if (i == 0) { soCho = 1; trangThai = "MoDangKy"; }        // chỉ còn 1 chỗ (sẽ set 0 đăng ký hợp lệ ban đầu để test)
                else if (i == 1) { soCho = 2; trangThai = "MoDangKy"; }   // sẽ bị lấp đầy bởi đăng ký
                else if (i == 2) { trangThai = "TamKhoa"; }
                else if (i == 3) { trangThai = "KetThuc"; }
                else if (i >= 15) { soCho = 4; }

                deTais.Add(new DeTai
                {
                    TenDeTai = tenDeTaiMau[i],
                    LinhVuc = lv,
                    MoTa = "Mô tả chi tiết cho đề tài: " + tenDeTaiMau[i],
                    YeuCau = "Yêu cầu sinh viên nắm vững kiến thức liên quan lĩnh vực " + lv.TenLinhVuc,
                    SoSinhVienToiDa = soCho,
                    NamHoc = i < 10 ? 2025 : 2026,
                    HocKy = hocKyList[i % 2],
                    TrangThai = trangThai
                });
            }
            context.DeTais.AddRange(deTais);
            context.SaveChanges();

            // ===== 4. TÀI KHOẢN ADMIN (2) =====
            var admins = new List<TaiKhoan>
            {
                new() { TenDangNhap = "admin1", MatKhau = "admin123", HoTen = "Quản trị viên 1", Email = "admin1@uneti.edu.vn", VaiTro = "Admin", TrangThai = true },
                new() { TenDangNhap = "admin2", MatKhau = "admin123", HoTen = "Quản trị viên 2", Email = "admin2@uneti.edu.vn", VaiTro = "Admin", TrangThai = true },
            };
            context.TaiKhoans.AddRange(admins);
            context.SaveChanges();

            // ===== 5. SINH VIÊN + TÀI KHOẢN (30) =====
            string[] hoList = { "Nguyễn", "Trần", "Lê", "Phạm", "Hoàng", "Vũ", "Đặng", "Bùi", "Đỗ", "Ngô" };
            string[] tenList = { "Anh", "Bảo", "Chi", "Duy", "Giang", "Hà", "Khánh", "Linh", "Minh", "Ngọc", "Phong", "Quân", "Thảo", "Uyên", "Việt" };
            var sinhViens = new List<SinhVien>();

            for (int i = 1; i <= 30; i++)
            {
                var taiKhoan = new TaiKhoan
                {
                    TenDangNhap = "sv" + i.ToString("D3"),
                    MatKhau = "123456",
                    HoTen = hoList[i % hoList.Length] + " Văn " + tenList[i % tenList.Length],
                    Email = "sv" + i.ToString("D3") + "@student.uneti.edu.vn",
                    VaiTro = "SinhVien",
                    // 1 tài khoản bị khóa để test case "tài khoản bị khóa"
                    TrangThai = i != 30
                };
                context.TaiKhoans.Add(taiKhoan);
                context.SaveChanges();

                var sv = new SinhVien
                {
                    MaTaiKhoan = taiKhoan.MaTaiKhoan,
                    HoTen = taiKhoan.HoTen,
                    NgaySinh = new DateTime(2003, (i % 12) + 1, (i % 28) + 1),
                    Lop = "TI17A" + ((i % 3) + 1) + "HN",
                    Khoa = "Công nghệ thông tin",
                    ChuyenNganh = "Kỹ thuật phần mềm",
                    Email = taiKhoan.Email,
                    SoDienThoai = "09" + (20000000 + i).ToString(),
                    TrangThai = taiKhoan.TrangThai
                };
                sinhViens.Add(sv);
            }
            context.SinhViens.AddRange(sinhViens);
            context.SaveChanges();

            // ===== 6. ĐĂNG KÝ ĐỀ TÀI (27) =====
            var dangKys = new List<DangKyDeTai>();
            var rnd = new Random(42); // seed cố định để dữ liệu tái lập được

            // 15 đăng ký "Đã duyệt" (chỉ nhắm vào các đề tài đang Mở đăng ký để hợp lệ)
            var deTaiMoDangKy = deTais.Where(d => d.TrangThai == "MoDangKy").ToList();
            int svIndex = 0;

            for (int i = 0; i < 15 && svIndex < 25; i++, svIndex++)
            {
                var dt = deTaiMoDangKy[i % deTaiMoDangKy.Count];
                // Đề tài index 1 trong danh sách gốc (soCho=2) sẽ được lấp đầy đúng 2 đăng ký để test "Đủ số lượng"
                dangKys.Add(new DangKyDeTai
                {
                    SinhVien = sinhViens[svIndex],
                    DeTai = dt,
                    NgayDangKy = DateTime.Now.AddDays(-rnd.Next(5, 60)),
                    LyDoDangKy = "Em muốn tìm hiểu và phát triển đề tài này",
                    TrangThai = "DaDuyet",
                    GhiChuDuyet = "Đủ điều kiện, đồng ý duyệt"
                });
            }

            // 6 đăng ký "Chờ duyệt"
            for (int i = 0; i < 6 && svIndex < 30; i++, svIndex++)
            {
                dangKys.Add(new DangKyDeTai
                {
                    SinhVien = sinhViens[svIndex],
                    DeTai = deTaiMoDangKy[(i + 3) % deTaiMoDangKy.Count],
                    NgayDangKy = DateTime.Now.AddDays(-rnd.Next(1, 10)),
                    LyDoDangKy = "Đề tài phù hợp với định hướng của em",
                    TrangThai = "ChoDuyet"
                });
            }

            // 3 đăng ký "Từ chối"
            for (int i = 0; i < 3; i++)
            {
                dangKys.Add(new DangKyDeTai
                {
                    SinhVien = sinhViens[i],
                    DeTai = deTaiMoDangKy[(i + 5) % deTaiMoDangKy.Count],
                    NgayDangKy = DateTime.Now.AddDays(-rnd.Next(10, 30)),
                    LyDoDangKy = "Đăng ký thử",
                    TrangThai = "TuChoi",
                    GhiChuDuyet = "Sinh viên đã có đề tài khác đang hiệu lực"
                });
            }

            // 3 đăng ký "Đã hủy"
            for (int i = 3; i < 6; i++)
            {
                dangKys.Add(new DangKyDeTai
                {
                    SinhVien = sinhViens[i],
                    DeTai = deTaiMoDangKy[(i + 7) % deTaiMoDangKy.Count],
                    NgayDangKy = DateTime.Now.AddDays(-rnd.Next(10, 30)),
                    LyDoDangKy = "Đăng ký thử rồi đổi ý",
                    TrangThai = "DaHuy"
                });
            }

            context.DangKyDeTais.AddRange(dangKys);
            context.SaveChanges();

            // ===== 7. PHÂN CÔNG HƯỚNG DẪN =====
            var phanCongs = new List<PhanCongHuongDan>();
            var daDuyetList = dangKys.Where(d => d.TrangThai == "DaDuyet").ToList();

            // Đếm tải giả lập theo GV để cố tình cho 1 GV đạt giới hạn, 1 GV còn 1 suất
            var taiHienTai = giangViens.ToDictionary(g => g.MaGiangVien, g => 0);

            int gvIdx = 0;
            foreach (var dk in daDuyetList)
            {
                // Tìm giảng viên cùng lĩnh vực với đề tài, còn tải
                var ungVien = giangViens
                    .Where(g => g.LinhVuc!.MaLinhVuc == dk.DeTai!.MaLinhVuc
                                && g.TrangThai == "DangCongTac"
                                && taiHienTai[g.MaGiangVien] < g.SoSinhVienHuongDanToiDa)
                    .ToList();

                if (!ungVien.Any()) continue; // bỏ qua nếu lĩnh vực đó không còn GV rảnh (hiếm khi xảy ra với data này)

                var gv = ungVien[gvIdx % ungVien.Count];
                gvIdx++;

                phanCongs.Add(new PhanCongHuongDan
                {
                    DangKyDeTai = dk,
                    GiangVien = gv,
                    NgayBatDau = dk.NgayDangKy.AddDays(3),
                    NgayKetThuc = null,
                    TrangThai = "DangHuongDan",
                    GhiChu = "Phân công lần đầu"
                });
                taiHienTai[gv.MaGiangVien]++;
            }

            // Cố tình đẩy 1 giảng viên (GV đầu tiên trong lĩnh vực 0) đạt đúng giới hạn tải
            // bằng cách gán thêm phân công nếu còn đăng ký DaDuyet chưa được phân công
            var gvGioiHan = giangViens[0]; // SoSinhVienHuongDanToiDa = 4
            var dkChuaPhanCong = daDuyetList
                .Where(dk => !phanCongs.Any(pc => pc.DangKyDeTai == dk))
                .ToList();
            int slotsCanFill = gvGioiHan.SoSinhVienHuongDanToiDa - taiHienTai[gvGioiHan.MaGiangVien];
            for (int i = 0; i < slotsCanFill && i < dkChuaPhanCong.Count; i++)
            {
                phanCongs.Add(new PhanCongHuongDan
                {
                    DangKyDeTai = dkChuaPhanCong[i],
                    GiangVien = gvGioiHan,
                    NgayBatDau = DateTime.Now.AddDays(-10),
                    NgayKetThuc = null,
                    TrangThai = "DangHuongDan",
                    GhiChu = "Bổ sung để đạt giới hạn hướng dẫn (dữ liệu test)"
                });
                taiHienTai[gvGioiHan.MaGiangVien]++;
            }

            context.PhanCongHuongDans.AddRange(phanCongs);
            context.SaveChanges();

            // ===== 8. TRƯỜNG HỢP THAY ĐỔI GIẢNG VIÊN (lưu lịch sử) =====
            if (phanCongs.Any())
            {
                var pcCu = phanCongs[0];
                pcCu.TrangThai = "DaThayDoi";
                pcCu.NgayKetThuc = DateTime.Now.AddDays(-2);
                pcCu.GhiChu = "Thay đổi giảng viên hướng dẫn theo yêu cầu";

                var gvMoi = giangViens.First(g => g.MaGiangVien != pcCu.MaGiangVien
                    && g.LinhVuc!.MaLinhVuc == pcCu.DangKyDeTai!.DeTai!.MaLinhVuc
                    && g.TrangThai == "DangCongTac");

                var pcMoi = new PhanCongHuongDan
                {
                    DangKyDeTai = pcCu.DangKyDeTai,
                    GiangVien = gvMoi,
                    NgayBatDau = DateTime.Now.AddDays(-1),
                    NgayKetThuc = null,
                    TrangThai = "DangHuongDan",
                    GhiChu = "Phân công lại sau khi đổi GVHD"
                };
                context.PhanCongHuongDans.Add(pcMoi);
                context.SaveChanges();
            }
        }
    }
}