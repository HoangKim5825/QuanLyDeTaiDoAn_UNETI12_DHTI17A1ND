// Họ và tên: (Điền họ tên của bạn)
// Mã sinh viên: (Điền mã sinh viên của bạn)
// Nội dung thực hiện: Module 3 - Đăng ký đề tài, kiểm tra điều kiện đăng ký,
// hủy đăng ký (khi còn Chờ duyệt), theo dõi kết quả đăng ký của sinh viên

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDeTaiDoAn.Common;
using QuanLyDeTaiDoAn.Data;
using QuanLyDeTaiDoAn.Filters;
using QuanLyDeTaiDoAn.Models;
using QuanLyDeTaiDoAn.ViewModels;

namespace QuanLyDeTaiDoAn.Controllers
{
    [RequireLogin]
    [RequireRole(AppConstants.VaiTro_SinhVien)]
    public class DangKyDeTaiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DangKyDeTaiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // Hàm dùng chung: xác định Sinh viên đang đăng nhập TỪ SESSION
        // (không bao giờ lấy MaSinhVien từ tham số do người dùng gửi lên,
        // để không cho đổi ID trên URL nhằm xem/sửa dữ liệu người khác - mục 7.5)
        // =========================================================
        private async Task<SinhVien> LaySinhVienDangDangNhapAsync()
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan == null) return null;
            return await _context.SinhViens.FirstOrDefaultAsync(sv => sv.MaTaiKhoan == maTaiKhoan);
        }

        // =========================================================
        // Hàm dùng chung: TÍNH SỐ CHỖ CÒN của 1 đề tài bằng LINQ.
        // SoChoCon = SoSinhVienToiDa - SoSinhVienDangKyHopLe (mục 6.7).
        // Chỉ đếm đăng ký có trạng thái "Đã duyệt" (Từ chối/Đã hủy KHÔNG tính).
        // =========================================================
        private async Task<int> TinhSoChoConAsync(DeTai deTai)
        {
            int soDaDangKyHopLe = await _context.DangKyDeTais
                .CountAsync(dk => dk.MaDeTai == deTai.MaDeTai &&
                                   AppConstants.DangKy_TrangThaiHopLeDeTinhCho.Contains(dk.TrangThai));

            return deTai.SoSinhVienToiDa - soDaDangKyHopLe;
        }

        // =========================================================
        // 7.5 THEO DÕI KẾT QUẢ ĐĂNG KÝ - danh sách đăng ký của CHÍNH sinh viên
        // GET: /DangKyDeTai
        // =========================================================
        public async Task<IActionResult> Index()
        {
            var sinhVien = await LaySinhVienDangDangNhapAsync();
            if (sinhVien == null) return NotFound();

            var danhSach = await _context.DangKyDeTais
                .Include(dk => dk.DeTai).ThenInclude(dt => dt.LinhVuc)
                .Where(dk => dk.MaSinhVien == sinhVien.MaSinhVien)
                .OrderByDescending(dk => dk.NgayDangKy)
                .ToListAsync();

            var ketQua = new List<KetQuaDangKyViewModel>();
            foreach (var dk in danhSach)
            {
                // Tìm giảng viên hướng dẫn HIỆN TẠI (nếu có) - bản ghi PhanCongHuongDan
                // đang ở trạng thái "Đang hướng dẫn" ứng với đăng ký này.
                var phanCongHienTai = await _context.PhanCongHuongDans
                    .Include(pc => pc.GiangVien)
                    .Where(pc => pc.MaDangKy == dk.MaDangKy && pc.TrangThai == AppConstants.PhanCong_DangHuongDan)
                    .FirstOrDefaultAsync();

                ketQua.Add(new KetQuaDangKyViewModel
                {
                    MaDangKy = dk.MaDangKy,
                    TenDeTai = dk.DeTai.TenDeTai,
                    TenLinhVuc = dk.DeTai.LinhVuc?.TenLinhVuc,
                    NgayDangKy = dk.NgayDangKy,
                    TrangThai = dk.TrangThai,
                    GhiChuDuyet = dk.GhiChuDuyet,
                    TenGiangVienHuongDan = phanCongHienTai?.GiangVien?.HoTen
                });
            }

            return View(ketQua);
        }

        // GET: /DangKyDeTai/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var sinhVien = await LaySinhVienDangDangNhapAsync();
            if (sinhVien == null) return NotFound();

            var dangKy = await _context.DangKyDeTais
                .Include(dk => dk.DeTai).ThenInclude(dt => dt.LinhVuc)
                .FirstOrDefaultAsync(dk => dk.MaDangKy == id);

            if (dangKy == null) return NotFound();

            // Chỉ được xem đăng ký của CHÍNH MÌNH
            if (dangKy.MaSinhVien != sinhVien.MaSinhVien)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            return View(dangKy);
        }

        // =========================================================
        // 7.3 ĐĂNG KÝ ĐỀ TÀI - bước 1: danh sách đề tài đang mở để chọn
        // GET: /DangKyDeTai/DanhSachDeTai
        // =========================================================
        public async Task<IActionResult> DanhSachDeTai(string tuKhoa)
        {
            var query = _context.DeTais
                .Include(dt => dt.LinhVuc)
                .Where(dt => dt.TrangThai == AppConstants.DeTai_MoDangKy &&
                             dt.NamHoc == AppConstants.NamHocHienTai &&
                             dt.HocKy == AppConstants.HocKyHienTai)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                query = query.Where(dt => dt.TenDeTai.Contains(tuKhoa));
            }
            ViewBag.TuKhoa = tuKhoa;

            var deTais = await query.OrderBy(dt => dt.TenDeTai).ToListAsync();

            var ketQua = new List<DeTaiKhaDungViewModel>();
            foreach (var dt in deTais)
            {
                ketQua.Add(new DeTaiKhaDungViewModel
                {
                    MaDeTai = dt.MaDeTai,
                    TenDeTai = dt.TenDeTai,
                    TenLinhVuc = dt.LinhVuc?.TenLinhVuc,
                    NamHoc = dt.NamHoc,
                    HocKy = dt.HocKy,
                    SoSinhVienToiDa = dt.SoSinhVienToiDa,
                    SoChoCon = await TinhSoChoConAsync(dt),
                    TrangThai = dt.TrangThai
                });
            }

            return View(ketQua);
        }

        // GET: /DangKyDeTai/DangKy/5 -> hiển thị form xác nhận đăng ký
        public async Task<IActionResult> DangKy(int id)
        {
            var deTai = await _context.DeTais.Include(dt => dt.LinhVuc).FirstOrDefaultAsync(dt => dt.MaDeTai == id);
            if (deTai == null) return NotFound();

            ViewBag.SoChoCon = await TinhSoChoConAsync(deTai);
            return View(deTai);
        }

        // POST: /DangKyDeTai/DangKy/5 -> tạo đăng ký sau khi kiểm tra đầy đủ điều kiện
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKy(int id, string lyDoDangKy)
        {
            var sinhVien = await LaySinhVienDangDangNhapAsync();
            if (sinhVien == null) return NotFound();

            var deTai = await _context.DeTais.FirstOrDefaultAsync(dt => dt.MaDeTai == id);
            if (deTai == null) return NotFound();

            // ============ 7.3: KIỂM TRA ĐIỀU KIỆN ĐĂNG KÝ BẰNG EF CORE/LINQ ============

            // 1) Sinh viên đang hoạt động
            if (sinhVien.TrangThai != AppConstants.SinhVien_DangHoc)
            {
                ModelState.AddModelError(string.Empty, "Tài khoản sinh viên không ở trạng thái đang học, không thể đăng ký đề tài.");
            }

            // 2) Đề tài đang Mở đăng ký và đúng năm học/học kỳ hiện tại
            if (deTai.TrangThai != AppConstants.DeTai_MoDangKy ||
                deTai.NamHoc != AppConstants.NamHocHienTai ||
                deTai.HocKy != AppConstants.HocKyHienTai)
            {
                ModelState.AddModelError(string.Empty, "Đề tài hiện không mở đăng ký hoặc không thuộc đợt hiện tại.");
            }

            // 3) Đề tài còn khả năng tiếp nhận sinh viên (SoChoCon > 0)
            int soChoCon = await TinhSoChoConAsync(deTai);
            if (soChoCon <= 0)
            {
                ModelState.AddModelError(string.Empty, "Đề tài đã đủ số lượng sinh viên đăng ký.");
            }

            // 4) Sinh viên KHÔNG có một đăng ký "Đã duyệt" khác đang hiệu lực trong cùng đợt
            bool daCoDeTaiHieuLuc = await _context.DangKyDeTais
                .Include(dk => dk.DeTai)
                .AnyAsync(dk => dk.MaSinhVien == sinhVien.MaSinhVien &&
                                dk.TrangThai == AppConstants.DangKy_DaDuyet &&
                                dk.DeTai.NamHoc == AppConstants.NamHocHienTai &&
                                dk.DeTai.HocKy == AppConstants.HocKyHienTai);
            if (daCoDeTaiHieuLuc)
            {
                ModelState.AddModelError(string.Empty, "Bạn đã có một đề tài được duyệt/đang thực hiện trong đợt này, không thể đăng ký thêm.");
            }

            // 5) Không tạo nhiều đăng ký "Chờ duyệt" cho CÙNG một đề tài
            bool daCoChoDuyetTrungDeTai = await _context.DangKyDeTais
                .AnyAsync(dk => dk.MaSinhVien == sinhVien.MaSinhVien &&
                                dk.MaDeTai == deTai.MaDeTai &&
                                dk.TrangThai == AppConstants.DangKy_ChoDuyet);
            if (daCoChoDuyetTrungDeTai)
            {
                ModelState.AddModelError(string.Empty, "Bạn đã gửi đăng ký cho đề tài này và đang chờ duyệt, không cần đăng ký lại.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.SoChoCon = soChoCon;
                return View(await _context.DeTais.Include(dt => dt.LinhVuc).FirstOrDefaultAsync(dt => dt.MaDeTai == id));
            }

            // ============ Hợp lệ -> tạo đăng ký ============
            var dangKyMoi = new DangKyDeTai
            {
                MaSinhVien = sinhVien.MaSinhVien,
                MaDeTai = deTai.MaDeTai,
                NgayDangKy = DateTime.Now, // Do hệ thống xác định, KHÔNG cho người dùng tự nhập (mục 7.3)
                LyDoDangKy = lyDoDangKy,
                TrangThai = AppConstants.DangKy_ChoDuyet,
                GhiChuDuyet = null
            };

            _context.DangKyDeTais.Add(dangKyMoi);
            await _context.SaveChangesAsync();

            TempData["ThongBao"] = "Đăng ký đề tài thành công, vui lòng chờ Admin duyệt.";
            return RedirectToAction(nameof(Index));
        }

        // =========================================================
        // 7.4 HỦY ĐĂNG KÝ - chỉ khi đăng ký còn ở trạng thái "Chờ duyệt"
        // POST: /DangKyDeTai/Huy/5
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Huy(int id)
        {
            var sinhVien = await LaySinhVienDangDangNhapAsync();
            if (sinhVien == null) return NotFound();

            var dangKy = await _context.DangKyDeTais.FirstOrDefaultAsync(dk => dk.MaDangKy == id);
            if (dangKy == null) return NotFound();

            // Chỉ được hủy đăng ký của CHÍNH MÌNH
            if (dangKy.MaSinhVien != sinhVien.MaSinhVien)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            // Chỉ hủy được khi đang "Chờ duyệt"; đăng ký "Đã duyệt" phải giữ lại lịch sử (mục 7.4/7.6)
            if (dangKy.TrangThai != AppConstants.DangKy_ChoDuyet)
            {
                TempData["Loi"] = "Chỉ có thể hủy đăng ký khi đang ở trạng thái Chờ duyệt.";
                return RedirectToAction(nameof(Index));
            }

            dangKy.TrangThai = AppConstants.DangKy_DaHuy; // Soft-cancel: giữ bản ghi để có lịch sử
            await _context.SaveChangesAsync();

            TempData["ThongBao"] = "Hủy đăng ký thành công.";
            return RedirectToAction(nameof(Index));
        }
    }
}
