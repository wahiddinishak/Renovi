using Microsoft.EntityFrameworkCore.Migrations;

namespace renovi.Migrations
{
    public partial class AbsenStatusRemove : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "absenDetails");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "absenDetails",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);
        }
    }
}
