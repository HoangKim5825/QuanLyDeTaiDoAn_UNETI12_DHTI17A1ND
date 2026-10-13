//Họ và tên: <Nguyen Quang Huy>
// Mã sinh viên: <23203100080>
// Nội dung thực hiện: ViewModel danh sách/lọc/chi tiết giảng viên kèm tải hướng dẫn (Module 2)
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyDeTaiDoAn.Models;

namespace QuanLyDeTaiDoAn.ViewModels
{
    // Một dòng giảng viên. TaiHienTai được TÍNH từ phân công, không lưu DB.
    public class GiangVienListItem
    {
        public int MaGiangVien { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string? HocVi { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? SoDienThoai { get; set; }
        public int MaLinhVuc { get; set; }
        public string TenLinhVuc { get; set; } = string.Empty;
        public int SoSinhVienHuongDanToiDa { get; set; }
        public string TrangThai { get; set; } = string.Empty;

        public int TaiHienTai { get; set; }      // số phân công DangHuongDan
        public int TongPhanCong { get; set; }    // gồm cả lịch sử

        public bool ConTai => TaiHienTai < SoSinhVienHuongDanToiDa;
    }

    public class GiangVienFilterViewModel
    {
        public string? Keyword { get; set; }
        public int? MaLinhVuc { get; set; }
        public string? TrangThai { get; set; }
        public bool? ConTai { get; set; }        // true = còn tải, false = đã đạt giới hạn

        public List<GiangVienListItem> KetQua { get; set; } = new();
        public List<SelectListItem> LinhVucs { get; set; } = new();
        public List<SelectListItem> TrangThais { get; set; } = new();
    }

    public class GiangVienDetailsViewModel
    {
        public GiangVien GiangVien { get; set; } = new();
        public int TaiHienTai { get; set; }
        public int TongPhanCong { get; set; }
        public bool ConTai => TaiHienTai < GiangVien.SoSinhVienHuongDanToiDa;
    }
}
