using Microsoft.EntityFrameworkCore.Migrations;

namespace renovi.Migrations
{
    public partial class LaporanNote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "note",
                table: "laporans",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "note",
                table: "laporans");
        }
    }
}
