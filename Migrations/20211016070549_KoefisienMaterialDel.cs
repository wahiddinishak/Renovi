using Microsoft.EntityFrameworkCore.Migrations;

namespace renovi.Migrations
{
    public partial class KoefisienMaterialDel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "koefisien",
                table: "materials");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "koefisien",
                table: "materials",
                type: "double",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
