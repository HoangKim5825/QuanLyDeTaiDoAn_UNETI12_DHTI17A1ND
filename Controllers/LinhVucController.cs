// Họ và tên: Nguyễn Minh Đức
// Mã sinh viên: [23203100078]
// Nội dung thực hiện: Module 1 - ViewModel cho đăng nhập và quản lý tài khoản

using Microsoft.AspNetCore.Mvc;
using QuanLyDeTaiDoAn.Data;
using QuanLyDeTaiDoAn.Filters;
using QuanLyDeTaiDoAn.Models;

namespace QuanLyDeTaiDoAn.Controllers
{
    [YeuCauAdmin]
    public class LinhVucController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LinhVucController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string? keyword)
        {
            var query = _context.LinhVucs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                query = query.Where(l => l.TenLinhVuc.Contains(keyword));
            }

            ViewBag.Keyword = keyword;
            var list = query.OrderBy(l => l.TenLinhVuc).ToList();
            return View(list);
        }

        public IActionResult Details(int id)
        {
            var item = _context.LinhVucs.FirstOrDefault(l => l.MaLinhVuc == id);
            if (item == null) return NotFound();

            ViewBag.SoGiangVien = _context.GiangViens.Count(g => g.MaLinhVuc == id);
            ViewBag.SoDeTai = _context.DeTais.Count(d => d.MaLinhVuc == id);
            return View(item);
        }

        public IActionResult Create() => View(new LinhVuc());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(LinhVuc model)
        {
            if (_context.LinhVucs.Any(l => l.TenLinhVuc == model.TenLinhVuc))
                ModelState.AddModelError(nameof(model.TenLinhVuc), "Tên lĩnh vực đã tồn tại.");

            if (!ModelState.IsValid) return View(model);

            // Chỉ tạo đúng các cột thuộc Module 1, không đụng navigation collection
            var linhVuc = new LinhVuc
            {
                TenLinhVuc = model.TenLinhVuc,
                MoTa = model.MoTa,
                TrangThai = model.TrangThai
            };

            _context.LinhVucs.Add(linhVuc);
            _context.SaveChanges();

            TempData["Success"] = "Thêm lĩnh vực thành công.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var item = _context.LinhVucs.FirstOrDefault(l => l.MaLinhVuc == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, LinhVuc model)
        {
            if (id != model.MaLinhVuc) return NotFound();

            if (_context.LinhVucs.Any(l => l.TenLinhVuc == model.TenLinhVuc && l.MaLinhVuc != id))
                ModelState.AddModelError(nameof(model.TenLinhVuc), "Tên lĩnh vực đã tồn tại.");

            if (!ModelState.IsValid) return View(model);

            var item = _context.LinhVucs.FirstOrDefault(l => l.MaLinhVuc == id);
            if (item == null) return NotFound();

            item.TenLinhVuc = model.TenLinhVuc;
            item.MoTa = model.MoTa;
            item.TrangThai = model.TrangThai;

            _context.SaveChanges();
            TempData["Success"] = "Cập nhật lĩnh vực thành công.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var item = _context.LinhVucs.FirstOrDefault(l => l.MaLinhVuc == id);
            if (item == null) return NotFound();

            ViewBag.SoGiangVien = _context.GiangViens.Count(g => g.MaLinhVuc == id);
            ViewBag.SoDeTai = _context.DeTais.Count(d => d.MaLinhVuc == id);
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var item = _context.LinhVucs.FirstOrDefault(l => l.MaLinhVuc == id);
            if (item == null) return NotFound();

            // QUAN TRỌNG: không cho xóa nếu Giảng viên/Đề tài (Module 2) đang dùng lĩnh vực này -
            // đúng yêu cầu đề bài mục 5.4/5.5, đồng thời tránh lỗi FK Restrict từ khung chung.
            bool dangDuocSuDung = _context.GiangViens.Any(g => g.MaLinhVuc == id)
                               || _context.DeTais.Any(d => d.MaLinhVuc == id);

            if (dangDuocSuDung)
            {
                TempData["Error"] = "Không thể xóa: lĩnh vực đang được Giảng viên hoặc Đề tài sử dụng. " +
                                     "Bạn có thể chuyển Trạng thái sang \"Ngừng áp dụng\" thay vì xóa.";
                return RedirectToAction(nameof(Index));
            }

            _context.LinhVucs.Remove(item);
            _context.SaveChanges();

            TempData["Success"] = "Đã xóa lĩnh vực.";
            return RedirectToAction(nameof(Index));
        }
    }
}
