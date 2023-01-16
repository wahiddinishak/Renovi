using Microsoft.EntityFrameworkCore.Migrations;

namespace renovi.Migrations
{
    public partial class AbsenIdProyek : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "idProyek",
                table: "absenDetails",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "idProyek",
                table: "absenDetails");
        }
    }
}
