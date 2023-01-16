using Microsoft.EntityFrameworkCore.Migrations;

namespace renovi.Migrations
{
    public partial class AbsenIdProyekDel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "idProyek",
                table: "absenDetails");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "idProyek",
                table: "absenDetails",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);
        }
    }
}
