using Microsoft.EntityFrameworkCore.Migrations;

namespace renovi.Migrations
{
    public partial class EntityJadwalEnhancement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "header",
                table: "jadwals",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "header",
                table: "jadwals");
        }
    }
}
