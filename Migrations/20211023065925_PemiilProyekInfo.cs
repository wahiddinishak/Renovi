using Microsoft.EntityFrameworkCore.Migrations;

namespace renovi.Migrations
{
    public partial class PemiilProyekInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "budget",
                table: "proyeks");

            migrationBuilder.DropColumn(
                name: "pemilik",
                table: "proyeks");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "budget",
                table: "proyeks",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "pemilik",
                table: "proyeks",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);
        }
    }
}
