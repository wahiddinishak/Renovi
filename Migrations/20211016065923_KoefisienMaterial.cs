using Microsoft.EntityFrameworkCore.Migrations;

namespace renovi.Migrations
{
    public partial class KoefisienMaterial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "koefisien",
                table: "materials",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "koefisien",
                table: "materials");
        }
    }
}
