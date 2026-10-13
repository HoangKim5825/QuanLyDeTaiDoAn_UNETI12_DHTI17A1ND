// Họ và tên: <Nguyen Quang Huy>
// Mã sinh viên: <23203100080>
// Nội dung thực hiện: Chức năng nâng cao - xuất danh sách đề tài, giảng viên ra Excel (Module 2)
// Cần cài gói: dotnet add package ClosedXML
using ClosedXML.Excel;
using QuanLyDeTaiDoAn.Constants;
using QuanLyDeTaiDoAn.ViewModels;

namespace QuanLyDeTaiDoAn.Extensions
{
    public static class ExcelHelper
    {
        public static byte[] XuatDeTai(IReadOnlyList<DeTaiListItem> ds)
        {
            string[] tieuDe = { "STT", "Tên đề tài", "Lĩnh vực", "Năm học", "Học kỳ",
                                "SV tối đa", "Đã duyệt", "Chỗ còn", "Trạng thái" };

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Đề tài");
            VietTieuDe(ws, tieuDe);

            int r = 2;
            foreach (var d in ds)
            {
                ws.Cell(r, 1).SetValue(r - 1);
                ws.Cell(r, 2).SetValue(d.TenDeTai);
                ws.Cell(r, 3).SetValue(d.TenLinhVuc);
                ws.Cell(r, 4).SetValue(d.NamHoc);
                ws.Cell(r, 5).SetValue(d.HocKy);
                ws.Cell(r, 6).SetValue(d.SoSinhVienToiDa);
                ws.Cell(r, 7).SetValue(d.SoDaDangKy);
                ws.Cell(r, 8).SetValue(d.SoChoCon);
                ws.Cell(r, 9).SetValue(TrangThaiConst.DeTaiTT.Nhan(d.TrangThaiHienThi));
                r++;
            }
            return Luu(wb, ws);
        }

        public static byte[] XuatGiangVien(IReadOnlyList<GiangVienListItem> ds)
        {
            string[] tieuDe = { "STT", "Họ và tên", "Học vị", "Email", "Số điện thoại",
                                "Lĩnh vực", "Giới hạn", "Đang hướng dẫn", "Trạng thái" };

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Giảng viên");
            VietTieuDe(ws, tieuDe);

            int r = 2;
            foreach (var g in ds)
            {
                ws.Cell(r, 1).SetValue(r - 1);
                ws.Cell(r, 2).SetValue(g.HoTen);
                ws.Cell(r, 3).SetValue(g.HocVi ?? "");
                ws.Cell(r, 4).SetValue(g.Email);
                ws.Cell(r, 5).SetValue(g.SoDienThoai ?? "");   // dạng chữ để giữ số 0 đầu
                ws.Cell(r, 6).SetValue(g.TenLinhVuc);
                ws.Cell(r, 7).SetValue(g.SoSinhVienHuongDanToiDa);
                ws.Cell(r, 8).SetValue(g.TaiHienTai);
                ws.Cell(r, 9).SetValue(TrangThaiConst.GiangVienTT.Nhan(g.TrangThai));
                r++;
            }
            return Luu(wb, ws);
        }

        private static void VietTieuDe(IXLWorksheet ws, string[] tieuDe)
        {
            for (int i = 0; i < tieuDe.Length; i++)
                ws.Cell(1, i + 1).SetValue(tieuDe[i]);

            var hang = ws.Range(1, 1, 1, tieuDe.Length);
            hang.Style.Font.Bold = true;
            hang.Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.SheetView.FreezeRows(1);
        }

        private static byte[] Luu(XLWorkbook wb, IXLWorksheet ws)
        {
            ws.Columns().AdjustToContents();
            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return ms.ToArray();
        }
    }
}
