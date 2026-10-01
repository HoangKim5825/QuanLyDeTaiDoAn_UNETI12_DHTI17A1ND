// Họ và tên: Hoàng Kim
// Mã sinh viên: <23203100056>
// Nội dung thực hiện: Module 4 - Hàm nghiệp vụ bằng LINQ: tính tải giảng viên,
// tính số chỗ còn, kiểm tra điều kiện duyệt đăng ký và phân công/thay đổi GVHD.

using Microsoft.EntityFrameworkCore;
using QuanLyDeTaiDoAn.Data;
using QuanLyDeTaiDoAn.Helpers;
using QuanLyDeTaiDoAn.ViewModels;

namespace QuanLyDeTaiDoAn.Services
{
    // Kết quả kiểm tra: gom TẤT CẢ lỗi để hiển thị một lần cho người dùng
    public class KetQuaKiemTra
    {
        public List<string> Loi { get; } = new();
        public bool HopLe => Loi.Count == 0;
        public void Them(string loi) => Loi.Add(loi);
    }

    public class NghiepVuService
    {
        private readonly ApplicationDbContext _context;

        public NghiepVuService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // 1. TẢI HIỆN TẠI CỦA GIẢNG VIÊN (mục 8.4)
        //    = số phân công có trạng thái "DangHuongDan"
        //    DaThayDoi / KetThuc KHÔNG được tính.
        // ============================================================
        public Task<int> TinhTaiGiangVienAsync(int maGiangVien)
        {
            return _context.PhanCongHuongDans
                .CountAsync(p => p.MaGiangVien == maGiangVien
                              && p.TrangThai == TrangThaiPhanCong.DangHuongDan);
        }

        // ============================================================
        // 2. SỐ CHỖ CÒN CỦA ĐỀ TÀI (mục 6.7)
        //    = SoSinhVienToiDa - số đăng ký "DaDuyet"
        //    Trả về null nếu không tìm thấy đề tài.
        //    ChoDuyet / TuChoi / DaHuy không chiếm chỗ.
        // ============================================================
        public async Task<int?> TinhSoChoConAsync(int maDeTai)
        {
            var kq = await _context.DeTais
                .Where(d => d.MaDeTai == maDeTai)
                .Select(d => new
                {
                    d.SoSinhVienToiDa,
                    SoDaDuyet = d.DanhSachDangKy.Count(dk => dk.TrangThai == TrangThaiDangKy.DaDuyet)
                })
                .FirstOrDefaultAsync();

            return kq == null ? null : kq.SoSinhVienToiDa - kq.SoDaDuyet;
        }

        // ============================================================
        // 3. KIỂM TRA ĐIỀU KIỆN DUYỆT ĐĂNG KÝ (mục 8.1)
        //    Gọi lại tại thời điểm duyệt, không tin dữ liệu lúc sinh viên đăng ký.
        // ============================================================
        public async Task<KetQuaKiemTra> KiemTraDieuKienDuyetAsync(int maDangKy)
        {
            var kq = new KetQuaKiemTra();

            var dk = await _context.DangKyDeTais
                .Include(x => x.SinhVien)
                .Include(x => x.DeTai)
                .FirstOrDefaultAsync(x => x.MaDangKy == maDangKy);

            if (dk == null)
            {
                kq.Them("Không tìm thấy đăng ký.");
                return kq;
            }

            // (d) Đăng ký chưa bị hủy / xử lý trước đó -> tránh duyệt 2 lần
            if (dk.TrangThai != TrangThaiDangKy.ChoDuyet)
                kq.Them($"Đăng ký này đã được xử lý trước đó (trạng thái hiện tại: {dk.TrangThai}).");

            // (a) Sinh viên vẫn đang hoạt động
            if (dk.SinhVien == null || !dk.SinhVien.TrangThai)
                kq.Them("Sinh viên không còn ở trạng thái hoạt động.");

            // (b) Đề tài còn nhận sinh viên và còn chỗ
            if (dk.DeTai == null)
            {
                kq.Them("Không tìm thấy đề tài của đăng ký.");
                return kq;
            }

            if (dk.DeTai.TrangThai == TrangThaiDeTai.TamKhoa || dk.DeTai.TrangThai == TrangThaiDeTai.KetThuc)
                kq.Them("Đề tài đang tạm khóa hoặc đã kết thúc, không thể duyệt thêm.");

            var soChoCon = await TinhSoChoConAsync(dk.MaDeTai);
            if (soChoCon == null || soChoCon <= 0)
                kq.Them("Đề tài đã đủ số lượng sinh viên.");

            // (c) Sinh viên chưa có đăng ký "DaDuyet" khác trong cùng đợt (cùng năm học + học kỳ)
            var daCoDeTaiKhac = await _context.DangKyDeTais.AnyAsync(x =>
                x.MaSinhVien == dk.MaSinhVien
                && x.MaDangKy != dk.MaDangKy
                && x.TrangThai == TrangThaiDangKy.DaDuyet
                && x.DeTai!.NamHoc == dk.DeTai.NamHoc
                && x.DeTai.HocKy == dk.DeTai.HocKy);
            if (daCoDeTaiKhac)
                kq.Them("Sinh viên đã có một đề tài khác đang hiệu lực trong cùng đợt.");

            return kq;
        }

        // ============================================================
        // 4. KIỂM TRA ĐIỀU KIỆN PHÂN CÔNG / THAY ĐỔI GVHD (mục 8.3, 8.5)
        //    - Phân công lần đầu: maPhanCongDangThayThe = null
        //    - Thay đổi GVHD: truyền MaPhanCong đang "DangHuongDan" cần thay
        // ============================================================
        public async Task<KetQuaKiemTra> KiemTraDieuKienPhanCongAsync(
            int maDangKy, int maGiangVien, int? maPhanCongDangThayThe = null)
        {
            var kq = new KetQuaKiemTra();

            var dk = await _context.DangKyDeTais
                .Include(x => x.SinhVien)
                .Include(x => x.DeTai)
                .FirstOrDefaultAsync(x => x.MaDangKy == maDangKy);

            if (dk == null)
            {
                kq.Them("Không tìm thấy đăng ký.");
                return kq;
            }

            // Chỉ đăng ký "DaDuyet" mới được phân công (TuChoi/DaHuy/ChoDuyet đều không)
            if (dk.TrangThai != TrangThaiDangKy.DaDuyet)
                kq.Them("Chỉ đăng ký đã duyệt mới được phân công giảng viên.");

            // Sinh viên và đề tài vẫn hợp lệ
            if (dk.SinhVien == null || !dk.SinhVien.TrangThai)
                kq.Them("Sinh viên không còn ở trạng thái hoạt động.");
            if (dk.DeTai == null || dk.DeTai.TrangThai == TrangThaiDeTai.KetThuc)
                kq.Them("Đề tài đã kết thúc hoặc không tồn tại.");

            // Giảng viên tồn tại và được phép hướng dẫn
            var gv = await _context.GiangViens.FirstOrDefaultAsync(g => g.MaGiangVien == maGiangVien);
            if (gv == null)
            {
                kq.Them("Không tìm thấy giảng viên.");
                return kq;
            }
            if (gv.TrangThai != TrangThaiGiangVien.DangCongTac)
                kq.Them("Giảng viên đang tạm ngừng hướng dẫn, không thể phân công mới.");

            // Đúng lĩnh vực
            if (dk.DeTai != null && gv.MaLinhVuc != dk.DeTai.MaLinhVuc)
                kq.Them("Lĩnh vực của giảng viên không phù hợp với lĩnh vực của đề tài.");

            // Phân công đang hiệu lực của đăng ký này (tối đa 1)
            var pcHienTai = await _context.PhanCongHuongDans.FirstOrDefaultAsync(p =>
                p.MaDangKy == maDangKy && p.TrangThai == TrangThaiPhanCong.DangHuongDan);

            if (maPhanCongDangThayThe == null)
            {
                // Phân công lần đầu: đăng ký chưa được có phân công đang hiệu lực
                if (pcHienTai != null)
                    kq.Them("Đăng ký này đã có giảng viên hướng dẫn đang hiệu lực. Hãy dùng chức năng Thay đổi GVHD.");
            }
            else
            {
                // Thay đổi GVHD: phải đúng bản ghi đang hiệu lực và GV mới phải khác GV cũ
                if (pcHienTai == null || pcHienTai.MaPhanCong != maPhanCongDangThayThe)
                    kq.Them("Phân công cần thay đổi không còn hiệu lực.");
                else if (pcHienTai.MaGiangVien == maGiangVien)
                    kq.Them("Giảng viên mới trùng với giảng viên đang hướng dẫn.");
            }

            // Tải: SoSinhVienDangHuongDan < SoSinhVienHuongDanToiDa
            var tai = await TinhTaiGiangVienAsync(maGiangVien);
            if (tai >= gv.SoSinhVienHuongDanToiDa)
                kq.Them($"Giảng viên đã đạt giới hạn hướng dẫn ({tai}/{gv.SoSinhVienHuongDanToiDa}).");

            return kq;
        }

        // ============================================================
        // 5. DANH SÁCH GIẢNG VIÊN HỢP LỆ ĐỂ ĐỔ DROPDOWN (View PhanCong / ThayDoiGVHD)
        //    Lọc trong Controller trước khi ra View: đúng lĩnh vực, đang công tác, còn tải.
        // ============================================================
        public async Task<List<GiangVienTaiItem>> LayGiangVienPhuHopAsync(
            int maDangKy, int? loaiTruMaGiangVien = null)
        {
            var maLinhVuc = await _context.DangKyDeTais
                .Where(dk => dk.MaDangKy == maDangKy)
                .Select(dk => (int?)dk.DeTai!.MaLinhVuc)
                .FirstOrDefaultAsync();

            if (maLinhVuc == null) return new List<GiangVienTaiItem>();

            return await _context.GiangViens
                .Where(g => g.MaLinhVuc == maLinhVuc
                         && g.TrangThai == TrangThaiGiangVien.DangCongTac
                         && (loaiTruMaGiangVien == null || g.MaGiangVien != loaiTruMaGiangVien)
                         && g.DanhSachPhanCong.Count(p => p.TrangThai == TrangThaiPhanCong.DangHuongDan)
                            < g.SoSinhVienHuongDanToiDa)
                .Select(g => new GiangVienTaiItem
                {
                    MaGiangVien = g.MaGiangVien,
                    HoTen = g.HoTen,
                    TenLinhVuc = g.LinhVuc!.TenLinhVuc,
                    SoToiDa = g.SoSinhVienHuongDanToiDa,
                    SoDangHuongDan = g.DanhSachPhanCong.Count(p => p.TrangThai == TrangThaiPhanCong.DangHuongDan)
                })
                .OrderBy(x => x.HoTen)
                .ToListAsync();
        }
    }
}