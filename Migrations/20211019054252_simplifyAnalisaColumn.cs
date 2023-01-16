using Microsoft.EntityFrameworkCore.Migrations;

namespace renovi.Migrations
{
    public partial class simplifyAnalisaColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "harga",
                table: "analisas");

            migrationBuilder.DropColumn(
                name: "item",
                table: "analisas");

            migrationBuilder.DropColumn(
                name: "jenisItem",
                table: "analisas");

            migrationBuilder.DropColumn(
                name: "satuan",
                table: "analisas");

            migrationBuilder.AddColumn<int>(
                name: "idMaterial",
                table: "analisas",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "idMaterial",
                table: "analisas");

            migrationBuilder.AddColumn<double>(
                name: "harga",
                table: "analisas",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "item",
                table: "analisas",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "jenisItem",
                table: "analisas",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "satuan",
                table: "analisas",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);
        }
    }
}
