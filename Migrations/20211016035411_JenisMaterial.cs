using Microsoft.EntityFrameworkCore.Migrations;

namespace renovi.Migrations
{
    public partial class JenisMaterial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "idAct",
                table: "materials");

            migrationBuilder.AddColumn<string>(
                name: "jenis",
                table: "materials",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "jenis",
                table: "materials");

            migrationBuilder.AddColumn<int>(
                name: "idAct",
                table: "materials",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
