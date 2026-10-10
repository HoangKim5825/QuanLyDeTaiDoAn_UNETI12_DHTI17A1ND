// Họ và tên: <Nguyen Quang Huy>
// Mã sinh viên: <23203100080>
// Nội dung thực hiện: ViewModel tìm kiếm, lọc, sắp xếp, phân trang đề tài (Module 2)
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuanLyDeTaiDoAn.ViewModels
{
    public class DeTaiFilterViewModel
    {
        // ===== Đầu vào (từ query string) =====
        public string? Keyword { get; set; }
        public int? MaLinhVuc { get; set; }
        public int? NamHoc { get; set; }
        public string? HocKy { get; set; }
        public string? TrangThai { get; set; }
        public bool? ConCho { get; set; }          // true = còn chỗ, false = hết chỗ
        public string? SortOrder { get; set; }     // ten_asc, ten_desc, max_asc, max_desc, cho_asc, cho_desc
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 8;

        // ===== Đầu ra =====
        public PagedResult<DeTaiListItem> KetQua { get; set; } = new();

        // ===== Dữ liệu cho dropdown lọc =====
        public List<SelectListItem> LinhVucs { get; set; } = new();
        public List<SelectListItem> NamHocs { get; set; } = new();
        public List<SelectListItem> HocKys { get; set; } = new();
        public List<SelectListItem> TrangThais { get; set; } = new();
        public List<SelectListItem> CacKieuSapXep { get; set; } = new();

        // Dùng cho asp-all-route-data ở link phân trang để GIỮ toàn bộ điều kiện
        public Dictionary<string, string?> RouteValues(int page) => new()
        {
            ["Keyword"] = Keyword,
            ["MaLinhVuc"] = MaLinhVuc?.ToString(),
            ["NamHoc"] = NamHoc?.ToString(),
            ["HocKy"] = HocKy,
            ["TrangThai"] = TrangThai,
            ["ConCho"] = ConCho?.ToString().ToLower(),
            ["SortOrder"] = SortOrder,
            ["PageSize"] = PageSize.ToString(),
            ["Page"] = page.ToString()
        };
    }
}
