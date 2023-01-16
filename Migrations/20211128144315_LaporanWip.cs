using Microsoft.EntityFrameworkCore.Migrations;

namespace renovi.Migrations
{
    public partial class LaporanWip : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "wip",
                table: "laporans",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "wip",
                table: "laporans");
        }
    }
}
