using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyDeTaiDoAn_UNETI12_DHTI17A1ND.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LinhVucs",
                columns: table => new
                {
                    MaLinhVuc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenLinhVuc = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinhVucs", x => x.MaLinhVuc);
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoans",
                columns: table => new
                {
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDangNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    VaiTro = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoans", x => x.MaTaiKhoan);
                });

            migrationBuilder.CreateTable(
                name: "DeTais",
                columns: table => new
                {
                    MaDeTai = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDeTai = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    MaLinhVuc = table.Column<int>(type: "int", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YeuCau = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoSinhVienToiDa = table.Column<int>(type: "int", nullable: false),
                    NamHoc = table.Column<int>(type: "int", nullable: false),
                    HocKy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeTais", x => x.MaDeTai);
                    table.ForeignKey(
                        name: "FK_DeTais_LinhVucs_MaLinhVuc",
                        column: x => x.MaLinhVuc,
                        principalTable: "LinhVucs",
                        principalColumn: "MaLinhVuc",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GiangViens",
                columns: table => new
                {
                    MaGiangVien = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HocVi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    MaLinhVuc = table.Column<int>(type: "int", nullable: false),
                    SoSinhVienHuongDanToiDa = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiangViens", x => x.MaGiangVien);
                    table.ForeignKey(
                        name: "FK_GiangViens_LinhVucs_MaLinhVuc",
                        column: x => x.MaLinhVuc,
                        principalTable: "LinhVucs",
                        principalColumn: "MaLinhVuc",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SinhViens",
                columns: table => new
                {
                    MaSinhVien = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Lop = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Khoa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ChuyenNganh = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SinhViens", x => x.MaSinhVien);
                    table.ForeignKey(
                        name: "FK_SinhViens_TaiKhoans_MaTaiKhoan",
                        column: x => x.MaTaiKhoan,
                        principalTable: "TaiKhoans",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DangKyDeTais",
                columns: table => new
                {
                    MaDangKy = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaSinhVien = table.Column<int>(type: "int", nullable: false),
                    MaDeTai = table.Column<int>(type: "int", nullable: false),
                    NgayDangKy = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LyDoDangKy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    GhiChuDuyet = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DangKyDeTais", x => x.MaDangKy);
                    table.ForeignKey(
                        name: "FK_DangKyDeTais_DeTais_MaDeTai",
                        column: x => x.MaDeTai,
                        principalTable: "DeTais",
                        principalColumn: "MaDeTai",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DangKyDeTais_SinhViens_MaSinhVien",
                        column: x => x.MaSinhVien,
                        principalTable: "SinhViens",
                        principalColumn: "MaSinhVien",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PhanCongHuongDans",
                columns: table => new
                {
                    MaPhanCong = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDangKy = table.Column<int>(type: "int", nullable: false),
                    MaGiangVien = table.Column<int>(type: "int", nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhanCongHuongDans", x => x.MaPhanCong);
                    table.ForeignKey(
                        name: "FK_PhanCongHuongDans_DangKyDeTais_MaDangKy",
                        column: x => x.MaDangKy,
                        principalTable: "DangKyDeTais",
                        principalColumn: "MaDangKy",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhanCongHuongDans_GiangViens_MaGiangVien",
                        column: x => x.MaGiangVien,
                        principalTable: "GiangViens",
                        principalColumn: "MaGiangVien",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DangKyDeTais_MaDeTai",
                table: "DangKyDeTais",
                column: "MaDeTai");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyDeTais_MaSinhVien",
                table: "DangKyDeTais",
                column: "MaSinhVien");

            migrationBuilder.CreateIndex(
                name: "IX_DeTais_MaLinhVuc",
                table: "DeTais",
                column: "MaLinhVuc");

            migrationBuilder.CreateIndex(
                name: "IX_GiangViens_Email",
                table: "GiangViens",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GiangViens_MaLinhVuc",
                table: "GiangViens",
                column: "MaLinhVuc");

            migrationBuilder.CreateIndex(
                name: "IX_LinhVucs_TenLinhVuc",
                table: "LinhVucs",
                column: "TenLinhVuc",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhanCongHuongDans_MaDangKy",
                table: "PhanCongHuongDans",
                column: "MaDangKy");

            migrationBuilder.CreateIndex(
                name: "IX_PhanCongHuongDans_MaGiangVien",
                table: "PhanCongHuongDans",
                column: "MaGiangVien");

            migrationBuilder.CreateIndex(
                name: "IX_SinhViens_Email",
                table: "SinhViens",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SinhViens_MaTaiKhoan",
                table: "SinhViens",
                column: "MaTaiKhoan",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoans_TenDangNhap",
                table: "TaiKhoans",
                column: "TenDangNhap",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PhanCongHuongDans");

            migrationBuilder.DropTable(
                name: "DangKyDeTais");

            migrationBuilder.DropTable(
                name: "GiangViens");

            migrationBuilder.DropTable(
                name: "DeTais");

            migrationBuilder.DropTable(
                name: "SinhViens");

            migrationBuilder.DropTable(
                name: "LinhVucs");

            migrationBuilder.DropTable(
                name: "TaiKhoans");
        }
    }
}
