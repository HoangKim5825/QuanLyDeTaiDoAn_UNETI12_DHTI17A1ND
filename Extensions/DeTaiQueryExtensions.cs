// Họ và tên: <ĐIỀN TÊN>
// Mã sinh viên: <ĐIỀN MÃ SV>
// Nội dung thực hiện: Công thức tính số chỗ còn dùng chung, sắp xếp, phân trang (Module 2)
using Microsoft.EntityFrameworkCore;
using QuanLyDeTaiDoAn.Constants;
using QuanLyDeTaiDoAn.Models;
using QuanLyDeTaiDoAn.ViewModels;

namespace QuanLyDeTaiDoAn.Extensions
{
    public static class DeTaiQueryExtensions
    {
        // CÔNG THỨC DUY NHẤT (M3, M4 dùng chung):
        // SoChoCon = SoSinhVienToiDa - số đăng ký DaDuyet
        public static IQueryable<DeTaiListItem> ToListItems(this IQueryable<DeTai> query)
        {
            return query.Select(d => new DeTaiListItem
            {
                MaDeTai = d.MaDeTai,
                TenDeTai = d.TenDeTai,
                MaLinhVuc = d.MaLinhVuc,
                TenLinhVuc = d.LinhVuc!.TenLinhVuc,
                NamHoc = d.NamHoc,
                HocKy = d.HocKy,
                SoSinhVienToiDa = d.SoSinhVienToiDa,
                TrangThai = d.TrangThai,
                SoDaDangKy = d.DanhSachDangKy
                    .Count(x => x.TrangThai == TrangThaiConst.DangKyTT.DaDuyet),
                SoChoCon = d.SoSinhVienToiDa - d.DanhSachDangKy
                    .Count(x => x.TrangThai == TrangThaiConst.DangKyTT.DaDuyet)
            });
        }

        // Đăng ký "hợp lệ" (đang chiếm chỗ) = DaDuyet. Dùng cho Create/Edit/Delete đề tài.
        public static IQueryable<DangKyDeTai> DangKyHopLe(this IQueryable<DangKyDeTai> q)
            => q.Where(x => x.TrangThai == TrangThaiConst.DangKyTT.DaDuyet);

        // Sắp xếp. Luôn thêm ThenBy MaDeTai để thứ tự ổn định khi phân trang.
        public static IQueryable<DeTaiListItem> ApplySort(this IQueryable<DeTaiListItem> q, string? sortOrder)
        {
            return sortOrder switch
            {
                "ten_desc" => q.OrderByDescending(x => x.TenDeTai).ThenBy(x => x.MaDeTai),
                "max_asc"  => q.OrderBy(x => x.SoSinhVienToiDa).ThenBy(x => x.MaDeTai),
                "max_desc" => q.OrderByDescending(x => x.SoSinhVienToiDa).ThenBy(x => x.MaDeTai),
                "cho_asc"  => q.OrderBy(x => x.SoChoCon).ThenBy(x => x.MaDeTai),
                "cho_desc" => q.OrderByDescending(x => x.SoChoCon).ThenBy(x => x.MaDeTai),
                _          => q.OrderBy(x => x.TenDeTai).ThenBy(x => x.MaDeTai)   // ten_asc mặc định
            };
        }

        // Đếm tổng -> chỉnh trang hợp lệ -> Skip/Take
        public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
            this IQueryable<T> q, int page, int pageSize)
        {
            if (pageSize < 1) pageSize = 8;
            int total = await q.CountAsync();
            int totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));
            page = Math.Clamp(page, 1, totalPages);

            var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<T>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalItems = total
            };
        }
    }
}
