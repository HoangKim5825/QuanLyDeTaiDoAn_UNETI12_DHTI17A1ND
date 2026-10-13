using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyDeTaiDoAn_UNETI12_DHTI17A1ND.Migrations
{
    /// <inheritdoc />
    public partial class ThemFileMoTaDeTai : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TenFileLuu",
                table: "DeTais",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenFileMoTa",
                table: "DeTais",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenFileLuu",
                table: "DeTais");

            migrationBuilder.DropColumn(
                name: "TenFileMoTa",
                table: "DeTais");
        }
    }
}
