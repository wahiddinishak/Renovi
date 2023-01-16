using Microsoft.EntityFrameworkCore.Migrations;

namespace renovi.Migrations
{
    public partial class LaporanActH : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "idActH",
                table: "laporans",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "idActH",
                table: "laporans");
        }
    }
}
