// Họ và tên: <ĐIỀN TÊN>
// Mã sinh viên: <ĐIỀN MÃ SV>
// Nội dung thực hiện: Module 3 - Kiểm tra điều kiện đăng ký đề tài bằng EF Core/LINQ
//                     (kiểm tra lại ở server tại thời điểm đăng ký, không chỉ ở giao diện)

using Microsoft.EntityFrameworkCore;
using QuanLyDeTaiDoAn.Data;
using QuanLyDeTaiDoAn.Helpers;

namespace QuanLyDeTaiDoAn.Services
{
    public class DangKyService
    {
        private readonly ApplicationDbContext _context;
        private readonly NghiepVuService _nghiepVu;
        private readonly IConfiguration _config;

        public DangKyService(ApplicationDbContext context, NghiepVuService nghiepVu, IConfiguration config)
        {
            _context = context;
            _nghiepVu = nghiepVu;
            _config = config;
        }

        // Gom TẤT CẢ lỗi để hiển thị một lần cho sinh viên (mục 7.3).
        public async Task<KetQuaKiemTra> KiemTraDieuKienDangKyAsync(int maSinhVien, int maDeTai)
        {
            var kq = new KetQuaKiemTra();

            // (1) Sinh viên tồn tại và đang hoạt động (cả hồ sơ lẫn tài khoản)
            var sv = await _context.SinhViens.AsNoTracking()
                .Include(s => s.TaiKhoan)
                .FirstOrDefaultAsync(s => s.MaSinhVien == maSinhVien);
            if (sv == null)
            {
                kq.Them("Không tìm thấy hồ sơ sinh viên.");
                return kq;
            }
            if (!sv.TrangThai || sv.TaiKhoan == null || !sv.TaiKhoan.TrangThai)
                kq.Them("Sinh viên không ở trạng thái hoạt động nên không thể đăng ký.");

            // (2) Đề tài tồn tại, đang Mở đăng ký, đúng đợt
            var dt = await _context.DeTais.AsNoTracking().FirstOrDefaultAsync(d => d.MaDeTai == maDeTai);
            if (dt == null)
            {
                kq.Them("Không tìm thấy đề tài.");
                return kq;
            }

            if (dt.TrangThai != TrangThaiDeTai.MoDangKy)
                kq.Them("Đề tài hiện không ở trạng thái Mở đăng ký.");

            // Đợt đăng ký đang áp dụng cấu hình ở appsettings.json (mục "DotDangKy").
            // Không cấu hình thì không giới hạn đợt.
            int? namHocApDung = _config.GetValue<int?>("DotDangKy:NamHoc");
            string? hocKyApDung = _config["DotDangKy:HocKy"];
            if (namHocApDung.HasValue && dt.NamHoc != namHocApDung.Value)
                kq.Them($"Đề tài thuộc năm học {dt.NamHoc}, không đúng đợt đăng ký đang áp dụng (năm học {namHocApDung}).");
            if (!string.IsNullOrEmpty(hocKyApDung) && dt.HocKy != hocKyApDung)
                kq.Them($"Đề tài thuộc {dt.HocKy}, không đúng đợt đăng ký đang áp dụng ({hocKyApDung}).");

            // (3) Còn chỗ: SoChoCon = SoSinhVienToiDa - số đăng ký DaDuyet (dùng chung công thức của Module 4)
            var soChoCon = await _nghiepVu.TinhSoChoConAsync(maDeTai);
            if (soChoCon == null || soChoCon <= 0)
                kq.Them("Đề tài đã đủ số lượng sinh viên.");

            // (4) Sinh viên chưa có đăng ký "DaDuyet" nào trong cùng đợt (cùng năm học + học kỳ)
            bool daCoDeTaiHieuLuc = await _context.DangKyDeTais.AnyAsync(x =>
                x.MaSinhVien == maSinhVien
                && x.TrangThai == TrangThaiDangKy.DaDuyet
                && x.DeTai!.NamHoc == dt.NamHoc
                && x.DeTai.HocKy == dt.HocKy);
            if (daCoDeTaiHieuLuc)
                kq.Them("Bạn đã có một đề tài đang hiệu lực trong cùng đợt nên không thể đăng ký thêm.");

            // (5) Không tạo nhiều đăng ký "ChoDuyet" cho cùng một đề tài
            bool trungChoDuyet = await _context.DangKyDeTais.AnyAsync(x =>
                x.MaSinhVien == maSinhVien
                && x.MaDeTai == maDeTai
                && x.TrangThai == TrangThaiDangKy.ChoDuyet);
            if (trungChoDuyet)
                kq.Them("Bạn đã có một đăng ký đang chờ duyệt cho đề tài này.");

            return kq;
        }
    }
}
