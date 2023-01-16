using Microsoft.EntityFrameworkCore.Migrations;

namespace renovi.Migrations
{
    public partial class profitHargaActD : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "harga",
                table: "actDs",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "profit",
                table: "actDs",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "harga",
                table: "actDs");

            migrationBuilder.DropColumn(
                name: "profit",
                table: "actDs");
        }
    }
}
