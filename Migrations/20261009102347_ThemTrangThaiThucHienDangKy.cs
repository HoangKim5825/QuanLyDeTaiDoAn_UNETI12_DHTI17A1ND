using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyDeTaiDoAn_UNETI12_DHTI17A1ND.Migrations
{
    /// <inheritdoc />
    public partial class ThemTrangThaiThucHienDangKy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TrangThaiThucHien",
                table: "DangKyDeTais",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "ChuaBatDau");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrangThaiThucHien",
                table: "DangKyDeTais");
        }
    }
}
