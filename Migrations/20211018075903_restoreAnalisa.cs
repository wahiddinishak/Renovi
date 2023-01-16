using Microsoft.EntityFrameworkCore.Migrations;

namespace renovi.Migrations
{
    public partial class restoreAnalisa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "harga",
                table: "analisas",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "satuan",
                table: "analisas",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "harga",
                table: "analisas");

            migrationBuilder.DropColumn(
                name: "satuan",
                table: "analisas");
        }
    }
}
