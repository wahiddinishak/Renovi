using Microsoft.EntityFrameworkCore.Migrations;

namespace renovi.Migrations
{
    public partial class delHargaActD1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "harga",
                table: "actDs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "harga",
                table: "actDs",
                type: "double",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
