using Microsoft.EntityFrameworkCore.Migrations;

namespace renovi.Migrations
{
    public partial class simplifyAnalisa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "hargaItem",
                table: "analisas");

            migrationBuilder.DropColumn(
                name: "namaitem",
                table: "analisas");

            migrationBuilder.DropColumn(
                name: "uomItem",
                table: "analisas");

            migrationBuilder.AddColumn<string>(
                name: "item",
                table: "analisas",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "item",
                table: "analisas");

            migrationBuilder.AddColumn<double>(
                name: "hargaItem",
                table: "analisas",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "namaitem",
                table: "analisas",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "uomItem",
                table: "analisas",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);
        }
    }
}
