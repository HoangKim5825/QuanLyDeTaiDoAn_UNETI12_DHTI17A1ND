// Họ và tên: Hoàng Kim
// Mã sinh viên: <23203100056>
// Nội dung thực hiện: Module 4 - Xuất Excel (ClosedXML) và PDF (QuestPDF)
// cho danh sách đăng ký và bảng thống kê. Dữ liệu lấy trực tiếp từ DB bằng LINQ.

using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using QuanLyDeTaiDoAn.Data;
using QuanLyDeTaiDoAn.Helpers;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuanLyDeTaiDoAn.Services
{
    public class DongDangKy
    {
        public int MaDangKy { get; set; }
        public int MaSinhVien { get; set; }
        public string TenSinhVien { get; set; } = "";
        public string TenDeTai { get; set; } = "";
        public string TenLinhVuc { get; set; } = "";
        public DateTime NgayDangKy { get; set; }
        public string TrangThai { get; set; } = "";
        public string TienDo { get; set; } = "";
        public string GiangVien { get; set; } = "";
        public string? GhiChuDuyet { get; set; }
    }

    public class XuatFileService
    {
        private readonly ApplicationDbContext _context;
        public XuatFileService(ApplicationDbContext context) => _context = context;

        // ---------- Nhãn tiếng Việt ----------
        private static string NhanDangKy(string t) => t switch
        {
            "ChoDuyet" => "Chờ duyệt",
            "DaDuyet" => "Đã duyệt",
            "TuChoi" => "Từ chối",
            "DaHuy" => "Đã hủy",
            _ => t
        };
        private static string NhanTienDo(string t) => t switch
        {
            "DangThucHien" => "Đang thực hiện",
            "HoanThanh" => "Hoàn thành",
            _ => "Chưa bắt đầu"
        };

        // ============================================================
        // DỮ LIỆU: danh sách đăng ký (cùng bộ lọc với trang DanhSachChoDuyet, không phân trang)
        // ============================================================
        public async Task<List<DongDangKy>> LayDanhSachDangKyAsync(
            string? tuKhoa, int? maLinhVuc, string trangThai)
        {
            var query = _context.DangKyDeTais.AsNoTracking().AsQueryable();

            if (trangThai != "TatCa")
                query = query.Where(dk => dk.TrangThai == trangThai);
            if (maLinhVuc.HasValue)
                query = query.Where(dk => dk.DeTai!.MaLinhVuc == maLinhVuc.Value);
            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                tuKhoa = tuKhoa.Trim();
                bool laSo = int.TryParse(tuKhoa, out int maSv);
                query = query.Where(dk =>
                    dk.SinhVien!.HoTen.Contains(tuKhoa)
                    || dk.DeTai!.TenDeTai.Contains(tuKhoa)
                    || (laSo && dk.MaSinhVien == maSv));
            }

            var raw = await query
                .OrderBy(dk => dk.NgayDangKy)
                .Select(dk => new DongDangKy
                {
                    MaDangKy = dk.MaDangKy,
                    MaSinhVien = dk.MaSinhVien,
                    TenSinhVien = dk.SinhVien!.HoTen,
                    TenDeTai = dk.DeTai!.TenDeTai,
                    TenLinhVuc = dk.DeTai.LinhVuc!.TenLinhVuc,
                    NgayDangKy = dk.NgayDangKy,
                    TrangThai = dk.TrangThai,
                    TienDo = dk.TrangThaiThucHien,
                    GhiChuDuyet = dk.GhiChuDuyet,
                    GiangVien = dk.DanhSachPhanCong
                        .Where(p => p.TrangThai == TrangThaiPhanCong.DangHuongDan)
                        .Select(p => p.GiangVien!.HoTen)
                        .FirstOrDefault() ?? ""
                })
                .ToListAsync();

            // Đổi sang nhãn tiếng Việt sau khi đã truy vấn xong
            foreach (var x in raw)
            {
                x.TrangThai = NhanDangKy(x.TrangThai);
                x.TienDo = NhanTienDo(x.TienDo);
            }
            return raw;
        }

        // ============================================================
        // DỮ LIỆU: các bảng thống kê (mỗi bảng = tiêu đề + header + các dòng)
        // ============================================================
        public class BangThongKe
        {
            public string TieuDe { get; set; } = "";
            public string[] Cot { get; set; } = Array.Empty<string>();
            public List<string[]> Dong { get; set; } = new();
        }

        public async Task<List<BangThongKe>> LayCacBangThongKeAsync()
        {
            var ds = new List<BangThongKe>();

            // 1. Đề tài theo lĩnh vực
            var a = await _context.DeTais.GroupBy(d => d.LinhVuc!.TenLinhVuc)
                .Select(g => new { g.Key, SL = g.Count() })
                .OrderByDescending(x => x.SL).ToListAsync();
            ds.Add(new BangThongKe
            {
                TieuDe = "Số đề tài theo lĩnh vực",
                Cot = new[] { "Lĩnh vực", "Số đề tài" },
                Dong = a.Select(x => new[] { x.Key, x.SL.ToString() }).ToList()
            });

            // 2. Đăng ký theo trạng thái
            var b = await _context.DangKyDeTais.GroupBy(d => d.TrangThai)
                .Select(g => new { g.Key, SL = g.Count() })
                .OrderByDescending(x => x.SL).ToListAsync();
            ds.Add(new BangThongKe
            {
                TieuDe = "Số đăng ký theo trạng thái",
                Cot = new[] { "Trạng thái", "Số đăng ký" },
                Dong = b.Select(x => new[] { NhanDangKy(x.Key), x.SL.ToString() }).ToList()
            });

            // 3. Đăng ký theo tháng
            var c = await _context.DangKyDeTais
                .GroupBy(d => new { d.NgayDangKy.Year, d.NgayDangKy.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, SL = g.Count() })
                .OrderBy(x => x.Year).ThenBy(x => x.Month).ToListAsync();
            ds.Add(new BangThongKe
            {
                TieuDe = "Số đăng ký theo tháng",
                Cot = new[] { "Tháng", "Số đăng ký" },
                Dong = c.Select(x => new[] { $"{x.Month:00}/{x.Year}", x.SL.ToString() }).ToList()
            });

            // 4. Đề tài theo năm học / học kỳ
            var d4 = await _context.DeTais.GroupBy(d => new { d.NamHoc, d.HocKy })
                .Select(g => new { g.Key.NamHoc, g.Key.HocKy, SL = g.Count() })
                .OrderBy(x => x.NamHoc).ThenBy(x => x.HocKy).ToListAsync();
            ds.Add(new BangThongKe
            {
                TieuDe = "Số đề tài theo năm học / học kỳ",
                Cot = new[] { "Năm học - Học kỳ", "Số đề tài" },
                Dong = d4.Select(x => new[] { $"{x.NamHoc} - {x.HocKy}", x.SL.ToString() }).ToList()
            });

            // 5. Số chỗ còn từng đề tài (tính lúc truy vấn)
            var e = await _context.DeTais.Select(d => new
            {
                d.TenDeTai,
                LinhVuc = d.LinhVuc!.TenLinhVuc,
                d.SoSinhVienToiDa,
                DaDuyet = d.DanhSachDangKy.Count(dk => dk.TrangThai == TrangThaiDangKy.DaDuyet)
            }).OrderBy(x => x.TenDeTai).ToListAsync();
            ds.Add(new BangThongKe
            {
                TieuDe = "Số sinh viên và số chỗ còn của từng đề tài",
                Cot = new[] { "Đề tài", "Lĩnh vực", "Tối đa", "Đã duyệt", "Còn chỗ" },
                Dong = e.Select(x => new[] { x.TenDeTai, x.LinhVuc,
                    x.SoSinhVienToiDa.ToString(), x.DaDuyet.ToString(),
                    (x.SoSinhVienToiDa - x.DaDuyet).ToString() }).ToList()
            });

            // 6. Tải & tỷ lệ sử dụng của giảng viên (chỉ đếm DangHuongDan)
            var f = await _context.GiangViens.Select(g => new
            {
                g.HoTen,
                LinhVuc = g.LinhVuc!.TenLinhVuc,
                g.SoSinhVienHuongDanToiDa,
                Tai = g.DanhSachPhanCong.Count(p => p.TrangThai == TrangThaiPhanCong.DangHuongDan)
            }).OrderByDescending(x => x.Tai).ThenBy(x => x.HoTen).ToListAsync();
            ds.Add(new BangThongKe
            {
                TieuDe = "Tải hướng dẫn và tỷ lệ sử dụng của giảng viên",
                Cot = new[] { "Giảng viên", "Lĩnh vực", "Đang hướng dẫn", "Tối đa", "Tỷ lệ (%)" },
                Dong = f.Select(x => new[] { x.HoTen, x.LinhVuc, x.Tai.ToString(),
                    x.SoSinhVienHuongDanToiDa.ToString(),
                    (x.SoSinhVienHuongDanToiDa > 0
                        ? Math.Round(x.Tai * 100.0 / x.SoSinhVienHuongDanToiDa, 1) : 0).ToString("0.0") }).ToList()
            });

            return ds;
        }

        // ============================================================
        // EXCEL (ClosedXML)
        // ============================================================
        public byte[] XuatDangKyExcel(List<DongDangKy> ds)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Danh sách đăng ký");

            string[] tieuDe = { "Mã ĐK", "Mã SV", "Sinh viên", "Đề tài", "Lĩnh vực",
                                "Ngày đăng ký", "Trạng thái", "Tiến độ", "GVHD hiện tại", "Ghi chú duyệt" };
            for (int i = 0; i < tieuDe.Length; i++) ws.Cell(1, i + 1).Value = tieuDe[i];
            DinhDangHeader(ws.Range(1, 1, 1, tieuDe.Length));

            int r = 2;
            foreach (var x in ds)
            {
                ws.Cell(r, 1).Value = x.MaDangKy;
                ws.Cell(r, 2).Value = x.MaSinhVien;
                ws.Cell(r, 3).Value = x.TenSinhVien;
                ws.Cell(r, 4).Value = x.TenDeTai;
                ws.Cell(r, 5).Value = x.TenLinhVuc;
                ws.Cell(r, 6).Value = x.NgayDangKy;
                ws.Cell(r, 6).Style.DateFormat.Format = "dd/MM/yyyy";
                ws.Cell(r, 7).Value = x.TrangThai;
                ws.Cell(r, 8).Value = x.TienDo;
                ws.Cell(r, 9).Value = x.GiangVien;
                ws.Cell(r, 10).Value = x.GhiChuDuyet ?? "";
                r++;
            }
            if (ds.Count > 0) ws.Range(1, 1, r - 1, tieuDe.Length).SetAutoFilter();
            ws.SheetView.FreezeRows(1);
            ws.Columns().AdjustToContents();

            return LuuByte(wb);
        }

        public byte[] XuatThongKeExcel(List<BangThongKe> cacBang)
        {
            using var wb = new XLWorkbook();
            int stt = 1;
            foreach (var b in cacBang)
            {
                // Tên sheet tối đa 31 ký tự, không chứa ký tự đặc biệt
                string ten = $"{stt++}. {b.TieuDe}";
                ten = new string(ten.Where(ch => !"[]:*?/\\".Contains(ch)).ToArray());
                var ws = wb.Worksheets.Add(ten.Length > 31 ? ten[..31] : ten);

                for (int i = 0; i < b.Cot.Length; i++) ws.Cell(1, i + 1).Value = b.Cot[i];
                DinhDangHeader(ws.Range(1, 1, 1, b.Cot.Length));

                for (int r = 0; r < b.Dong.Count; r++)
                    for (int c = 0; c < b.Dong[r].Length; c++)
                    {
                        // Ô số lưu dạng số để Excel tính toán được
                        var v = b.Dong[r][c];
                        if (double.TryParse(v, System.Globalization.NumberStyles.Any,
                                System.Globalization.CultureInfo.InvariantCulture, out var so) && c > 0 && !v.Contains('/'))
                            ws.Cell(r + 2, c + 1).Value = so;
                        else
                            ws.Cell(r + 2, c + 1).Value = v;
                    }
                ws.Columns().AdjustToContents();
            }
            return LuuByte(wb);
        }

        private static void DinhDangHeader(IXLRange h)
        {
            h.Style.Font.Bold = true;
            h.Style.Font.FontColor = XLColor.White;
            h.Style.Fill.BackgroundColor = XLColor.FromHtml("#1b6ec2");
            h.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }

        private static byte[] LuuByte(XLWorkbook wb)
        {
            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return ms.ToArray();
        }

        // ============================================================
        // PDF (QuestPDF)
        // ============================================================
        public byte[] XuatDangKyPdf(List<DongDangKy> ds, string moTaLoc)
        {
            return Document.Create(doc =>
            {
                doc.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(25);
                    page.DefaultTextStyle(t => t.FontSize(9).FontFamily("Arial"));

                    page.Header().Column(col =>
                    {
                        col.Item().Text("DANH SÁCH ĐĂNG KÝ ĐỀ TÀI").FontSize(15).Bold();
                        col.Item().Text($"{moTaLoc} • Xuất lúc {DateTime.Now:dd/MM/yyyy HH:mm} • Tổng: {ds.Count}")
                            .FontSize(9).FontColor(Colors.Grey.Darken1);
                    });

                    page.Content().PaddingTop(10).Table(t =>
                    {
                        t.ColumnsDefinition(c =>
                        {
                            c.ConstantColumn(30);    // Mã ĐK
                            c.RelativeColumn(2);     // Sinh viên
                            c.RelativeColumn(3);     // Đề tài
                            c.RelativeColumn(2);     // Lĩnh vực
                            c.ConstantColumn(60);    // Ngày
                            c.ConstantColumn(60);    // Trạng thái
                            c.RelativeColumn(2);     // GVHD
                            c.ConstantColumn(65);    // Tiến độ
                        });

                        t.Header(h =>
                        {
                            foreach (var s in new[] { "Mã", "Sinh viên", "Đề tài", "Lĩnh vực",
                                                      "Ngày ĐK", "Trạng thái", "GVHD", "Tiến độ" })
                                h.Cell().Background("#1b6ec2").Padding(4)
                                 .Text(s).FontColor(Colors.White).Bold();
                        });

                        foreach (var x in ds)
                        {
                            O(t, x.MaDangKy.ToString());
                            O(t, $"{x.TenSinhVien} ({x.MaSinhVien})");
                            O(t, x.TenDeTai);
                            O(t, x.TenLinhVuc);
                            O(t, x.NgayDangKy.ToString("dd/MM/yyyy"));
                            O(t, x.TrangThai);
                            O(t, string.IsNullOrEmpty(x.GiangVien) ? "—" : x.GiangVien);
                            O(t, x.TienDo);
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Trang "); x.CurrentPageNumber(); x.Span(" / "); x.TotalPages();
                    });
                });
            }).GeneratePdf();
        }

        public byte[] XuatThongKePdf(List<BangThongKe> cacBang)
        {
            return Document.Create(doc =>
            {
                doc.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(t => t.FontSize(10).FontFamily("Arial"));

                    page.Header().Column(col =>
                    {
                        col.Item().Text("BÁO CÁO THỐNG KÊ ĐỀ TÀI ĐỒ ÁN").FontSize(16).Bold();
                        col.Item().Text($"Xuất lúc {DateTime.Now:dd/MM/yyyy HH:mm}")
                            .FontSize(9).FontColor(Colors.Grey.Darken1);
                    });

                    page.Content().PaddingTop(10).Column(col =>
                    {
                        col.Spacing(14);
                        foreach (var b in cacBang)
                        {
                            col.Item().ShowEntire().Column(c =>
                            {
                                c.Item().PaddingBottom(4).Text(b.TieuDe).FontSize(12).Bold();
                                if (b.Dong.Count == 0)
                                {
                                    c.Item().Text("Chưa có dữ liệu.").FontColor(Colors.Grey.Darken1);
                                    return;
                                }
                                c.Item().Table(t =>
                                {
                                    t.ColumnsDefinition(cd =>
                                    {
                                        for (int i = 0; i < b.Cot.Length; i++)
                                            if (i == 0) cd.RelativeColumn(3); else cd.RelativeColumn(1.5f);
                                    });
                                    t.Header(h =>
                                    {
                                        foreach (var s in b.Cot)
                                            h.Cell().Background("#1b6ec2").Padding(4)
                                             .Text(s).FontColor(Colors.White).Bold();
                                    });
                                    foreach (var dong in b.Dong)
                                        foreach (var o in dong) O(t, o);
                                });
                            });
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Trang "); x.CurrentPageNumber(); x.Span(" / "); x.TotalPages();
                    });
                });
            }).GeneratePdf();
        }

        // Ô dữ liệu có viền mảnh
        private static void O(TableDescriptor t, string text) =>
            t.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(4).Text(text);
    }
}