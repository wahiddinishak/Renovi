using Microsoft.EntityFrameworkCore.Migrations;

namespace renovi.Migrations
{
    public partial class idActDJadwal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "idAct",
                table: "jadwals");

            migrationBuilder.AddColumn<int>(
                name: "idActD",
                table: "jadwals",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "idActD",
                table: "jadwals");

            migrationBuilder.AddColumn<int>(
                name: "idAct",
                table: "jadwals",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
